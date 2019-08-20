namespace EnterPlaymodeSetup.Examples.Editor
{
    using UnityEngine;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    public class AlwaysStartInSceneWithIndexZero : AbstractPlaymodeEnterInitializationLogic
    {
        private string[] _restoreScenePaths;

        public override void OnPreEnterPlaymode(Scene activeScene, Scene[] openScenes)
        {
            if (UnityEditor.EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogWarning(
                    "Build scenes are empty! Skipping AlwaysStartInSceneWithIndexZero initialization logic!");
                return;
            }

            _restoreScenePaths = new string[openScenes.Length];

            for (int i = 0; i < openScenes.Length; i++)
            {
                _restoreScenePaths[i] = openScenes[i].path;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(UnityEditor.EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
        }

        public override void OnPostExitPlaymode()
        {
            if (_restoreScenePaths == null)
                return;

            // restore the previous scenes
            EditorSceneManager.OpenScene(_restoreScenePaths[0], OpenSceneMode.Single);

            for (int i = 1; i < _restoreScenePaths.Length; i++)
            {
                EditorSceneManager.OpenScene(_restoreScenePaths[i], OpenSceneMode.Additive);
            }
        }
    }
}