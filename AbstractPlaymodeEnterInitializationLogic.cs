using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnterPlaymodeSetup
{
    public abstract class AbstractPlaymodeEnterInitializationLogic : ScriptableObject
    {
        public virtual void OnPreEnterPlaymode(Scene activeScene, Scene[] openScenes)
        {
            
        }

        public virtual void OnPostEnterPlaymode(Scene activeScene, Scene[] openScenes)
        {
            
        }

        public virtual void OnPostExitPlaymode()
        {
            
        }
    }
}