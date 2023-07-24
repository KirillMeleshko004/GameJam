using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Core.UI
{
    public class ToTitle : MonoBehaviour
    {
        public void ToTitleScreen()
        {
            SceneManager.LoadScene(0);
        }
    }
}
