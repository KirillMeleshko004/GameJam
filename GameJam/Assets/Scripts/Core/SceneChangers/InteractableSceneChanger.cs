using GameJam.Core.Interactions;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Core.SceneChangers
{
    public class InteractableSceneChanger : BaseSceneChanger, IInteractableSceneChanger
    {
        #region Variables
        [SerializeField]
        private Animator _balckoutAnimator;

        [SerializeField]
        private float _fadeOutAnimTime = 3f;

        [Header("Display, where interaction hint is showing")]
        [SerializeField]
        protected GameObject _hintDisplay;

        private bool _isUsed = false;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _changeSceneHint = "Press E to open door";
        #endregion

        #region Custom methods
        private void ChangeScene()
        {
            StartCoroutine(LoadSceneWithAnimation());
        }
        private IEnumerator LoadSceneWithAnimation()
        {
            _balckoutAnimator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(_fadeOutAnimTime);
            SceneManager.LoadScene(_targetSceneBuildID);
        }
        #endregion

        #region IInteractableSceneChanger implementation
        public GameObject InteractableObject { get { return this.gameObject; } }

        public void HideInteractionHint()
        {
            _hintDisplay.GetComponent<HintDisplay>().HideHint();
            _hintDisplay.SetActive(false);
        }

        public void Interact()
        {
            if(!_isUsed)
            {
                ChangeScene();
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                _isUsed = true;
            }
        }

        public void ShowInteractionHint()
        {
            _hintDisplay.SetActive(true);
            _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_changeSceneHint);
        }

        #endregion
    }
}
