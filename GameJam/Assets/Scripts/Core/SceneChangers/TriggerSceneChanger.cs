using UnityEngine.SceneManagement;

namespace GameJam.Core.SceneChangers
{
    //Scene changer, which changing scene when player enters designated zone
    public class TriggerSceneChanger : BaseSceneChanger, ITriggerSceneChanger
    {
        private void ChangeScene()
        {
            SceneManager.LoadScene(_targetSceneBuildID);
        }

        private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
        {
            if(collision.transform.root.CompareTag("Player"))
                ChangeScene();
        }
    }
}
