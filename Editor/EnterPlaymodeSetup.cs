namespace EnterPlaymodeSetup.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EnterPlaymodeSetup : ScriptableObject
    {
        private const string ASSET_TYPE_FILTER = "EnterPlaymodeSetup.Editor.EnterPlaymodeSetup";

        private static EnterPlaymodeSetup _instance
        {
            get
            {
                var instance = AssetDatabase.LoadAssetAtPath<EnterPlaymodeSetup>(AssetDatabase.FindAssets("t:" + ASSET_TYPE_FILTER)
                    .Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault());
                
                if (instance == null)
                {
                    instance = CreateInstance<EnterPlaymodeSetup>();

                    var scriptAsset = MonoScript.FromScriptableObject(instance);
                    var path = Path.Combine(
                        Path.GetDirectoryName(AssetDatabase.GetAssetPath(scriptAsset)),
                        "EnterPlaymodeSetup.asset");
                    
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                return instance;
            }
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            var inst = _instance;
            if (inst != null)
                EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }
        
        [SerializeField] private bool _isEnabled = true;

        [SerializeField] private List<MonoScript> _initializationLogic = new List<MonoScript>();

        [SerializeField, HideInInspector] private AbstractPlaymodeEnterInitializationLogic[] _logicInstances;

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange stateChange)
        {
            var inst = _instance;

            if (!inst._isEnabled)
                return;

            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                inst.OnPreEnterPlayMode();
            }
            else if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                inst.OnPostEnterPlayMode();
            }
            else if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                inst.OnEnterEditMode();
            }
        }

        private void OnPreEnterPlayMode()
        {
            // populate the instances
            _logicInstances = new AbstractPlaymodeEnterInitializationLogic[_initializationLogic.Count];

            for (int i = 0; i < _logicInstances.Length; i++)
            {
                _logicInstances[i] = (AbstractPlaymodeEnterInitializationLogic) CreateInstance(_initializationLogic[i].GetClass());

                AssetDatabase.AddObjectToAsset(_logicInstances[i], this);
            }

            var activeScene = SceneManager.GetActiveScene();
            var sceneCount = SceneManager.sceneCount;
            var openScenes = new Scene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                openScenes[i] = SceneManager.GetSceneAt(i);
            }

            for (int i = 0; i < _logicInstances.Length; i++)
            {
                _logicInstances[i].OnPreEnterPlaymode(activeScene, openScenes);
            }
        }

        private void OnPostEnterPlayMode()
        {
            var activeScene = SceneManager.GetActiveScene();
            var sceneCount = SceneManager.sceneCount;
            var openScenes = new Scene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                openScenes[i] = SceneManager.GetSceneAt(i);
            }

            for (int i = 0; i < _logicInstances.Length; i++)
            {
                if (_logicInstances[i] == null)
                    continue;
                _logicInstances[i].OnPostEnterPlaymode(activeScene, openScenes);
            }
        }

        private void OnEnterEditMode()
        {
            for (int i = 0; i < _logicInstances.Length; i++)
            {
                _logicInstances[i].OnPostExitPlaymode();
                DestroyImmediate(_logicInstances[i], true);
            }

            _logicInstances = null;
        }
        
        private void ValidateList()
        {
            _initializationLogic.RemoveAll((ms) =>
            {
                if (ms == null)
                {
                    Debug.LogWarning("Referenced script cannot be null!", this);
                    return true;
                }

                var classType = ms.GetClass();

                if (classType == null)
                {
                    Debug.LogWarning("Referenced script not recognized as a type! Did you name the script correctly?", this);
                    return true;
                }

                if (classType.IsAbstract)
                {
                    Debug.LogWarning("Referenced script must be a concrete implementation of AbstractPlaymodeEnterInitializationLogic", this);
                    return true;
                }

                if (typeof(AbstractPlaymodeEnterInitializationLogic).IsAssignableFrom(classType) == false)
                {
                    Debug.LogWarning("Referenced script has to implement the AbstractPlaymodeEnterInitializationLogic interface!", this);
                    return true;
                }

                return false;
            });
        }

        [CustomEditor(typeof(EnterPlaymodeSetup))]
        private class EnterPlaymodeSetupEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorGUILayout.Space();

                if (GUILayout.Button("Validate types"))
                {
                    ((EnterPlaymodeSetup) target).ValidateList();
                }
            }
        }
    }
}