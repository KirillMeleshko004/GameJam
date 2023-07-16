using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading;
using System.Collections;

namespace GameJam.Core.SceneChangers
{
    //Scene changer, which changing scene when player enters designated zone
    public class TriggerSceneChanger : BaseSceneChanger, ITriggerSceneChanger
    {
        #region Variables
        [SerializeField]
        private Animator _currentAnimator;

        [SerializeField]
        private float _fadeInAnimTime = 1f;
        [SerializeField]
        private float _fadeOutAnimTime = 3f;
        #endregion

        #region Built-in methods
        private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Player"))
                StartCoroutine(LoadSceneWithAnimation());
        }
        #endregion

        #region Custom methods
        private void ChangeScene()
        {
            _currentAnimator.SetTrigger("FadeIn");
            Debug.Log(_currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            SceneManager.LoadScene(_targetSceneBuildID);
        }

        private IEnumerator LoadSceneWithAnimation()
        {
            _currentAnimator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(_fadeOutAnimTime);
            SceneManager.LoadScene(_targetSceneBuildID);
        }
        #endregion
    }
}
