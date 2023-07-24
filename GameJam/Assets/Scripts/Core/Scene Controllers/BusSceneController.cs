using GameJam.Core.Dialogue;
using GameJam.Core.Interactions;
using GameJam.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Core.SceneControllers
{
    public class BusSceneController : MonoBehaviour
    {

        [SerializeField]
        private GameObject _player;
        [SerializeField]
        private GameObject _hintDisplay;
        [SerializeField]
        private string _continueInteractionHint = "Press E to Continue";

        [SerializeField]
        private DialogueScript _dialogueScript;
        [SerializeField]
        private TextAsset _dialogueText = null;
        [SerializeField]
        private float _startDialogueDelay;
        [SerializeField]
        private float _finishDialogueDelay;

        [SerializeField]
        private float _sceneLength;
        [SerializeField]
        private int _nextSceneId;

        [Header("Info for changing scene")]
        [SerializeField]
        private Animator _balckoutAnimator;
        [SerializeField]
        private float _fadeOutAnimTime = 3f;

        private bool _disableInteractions = true;


        private PlayerInput _input;

        void Start()
        {
            _player.GetComponent<Animator>().SetBool("isSittingOnChair", true);

            if (_dialogueText != null)
            {
                _input = _player.GetComponent<PlayerInput>();
                StartCoroutine(StartDialogue());
            }
            else StartCoroutine(LoadScene(_sceneLength));
        }


        void Update()
        {
            if (_dialogueText != null)
                HandleInteraction();
        }

        private void HandleInteraction()
        {
            if (_input.BasicInteractionInput)
                Interact();
        }

        private void Interact()
        {
            if (_disableInteractions) return;
            if (_dialogueScript.IsDialogueComleted)
            {
                _disableInteractions = true;
                HideInteractionHint();
                _dialogueScript.FinishDialogue();
                StartCoroutine(FinishInteraction());
            }

            else if (!_dialogueScript.IsStarted)
            {
                _dialogueScript.StartDialogue(_dialogueText);
            }

            else _dialogueScript.ShowNextSentence();
        }

        private IEnumerator StartDialogue()
        {
            yield return new WaitForSeconds(_startDialogueDelay);

            _disableInteractions = false;
            Interact();
            ShowInteractionHint();
        }

        private IEnumerator FinishInteraction()
        {
            yield return new WaitForSeconds(_finishDialogueDelay);
            StartCoroutine(LoadSceneWithAnimation());
        }

        private IEnumerator LoadSceneWithAnimation()
        {
            _balckoutAnimator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(_fadeOutAnimTime);
            SceneManager.LoadScene(_nextSceneId);
        }

        private IEnumerator LoadScene(float time)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(LoadSceneWithAnimation());
        }


        public void ShowInteractionHint()
        {

            _hintDisplay.SetActive(true);
            HintDisplay display = _hintDisplay.GetComponent<HintDisplay>();

            display.DisplayHint(_continueInteractionHint);
        }
        public void HideInteractionHint()
        {
            _hintDisplay.GetComponent<HintDisplay>().HideHint();
            _hintDisplay.SetActive(false);
        }
    }

}