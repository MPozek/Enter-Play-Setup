namespace EnterPlaymodeSetup.Examples
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LogPlaymodeEnterLogic : AbstractPlaymodeEnterInitializationLogic
    {
        public override void OnPreEnterPlaymode(Scene activeScene, Scene[] openScenes)
        {
            Debug.Log("Pre enter playmode!");
        }

        public override void OnPostEnterPlaymode(Scene activeScene, Scene[] openScenes)
        {
            Debug.Log("Post enter playmode!");
        }

        public override void OnPostExitPlaymode()
        {
            Debug.Log("Exit playmode!");
        }
    }
}