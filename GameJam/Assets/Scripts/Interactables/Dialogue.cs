using GameJam.Core.Interactions;
using GameJam.Inputs;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameJam.Items
{

    public class Dialogue : MonoBehaviour, IInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [Header("Display, where text is showing")]
        [SerializeField]
        private GameObject _dialogueDisplayPrefab;

        [SerializeField]
        private TextAsset _dialogueText;

        [Header("Display, where interaction hint is showing")]
        [SerializeField]
        protected GameObject _hintDisplay;

        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private float _sentenceTime;

        [SerializeField]
        private Vector3 _offset = Vector3.zero;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _startDialogueHint = "Press E to start dialogue";
        [SerializeField]
        private string _continueDialogueHint = "Press E to continue";


        private string[] _sentences;
        private TextMeshPro _textBox;

        private bool _isTypingNow = false;
        private bool _isDisplaying = false;
        private bool _isCompleted = false;
        private int _currentSentenceInd = 0;
        #endregion

        #region Built-in methods
        private void Awake()
        {
            _sentences = _dialogueText.text.Split('\n');
        }
        #endregion


        #region Custom methods
        private void ShowDialogue()
        {
            DisablePlayerMovement();
            _isDisplaying = true;
            GameObject obj = Instantiate(_dialogueDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.localPosition = _offset;
            _textBox = obj.transform.GetChild(0).GetComponentInChildren<TextMeshPro>();

            ShowNextSentence();
        }
        private void FinishDialogue()
        {
            GameObject.Destroy(_textBox.transform.parent.parent.gameObject);
            ResetParameters();
            EnablePlayerMovement();
        }
        private void ResetParameters()
        {
            _isTypingNow = false;
            _isDisplaying = false;
            _isCompleted = false;
            _currentSentenceInd = 0;
        }
        private IEnumerator WriteSentence(string sentence)
        {
            float charTime =  _sentenceTime / (float)sentence.Length;
            _isTypingNow = true;
            for(int i = 0; i < sentence.Length; i++)
            {
                _textBox.text = string.Concat(_textBox.text, sentence[i]);

                Debug.Log("Here");
                yield return new WaitForSeconds(charTime);
            }
            _isTypingNow = false;
            _currentSentenceInd++;
            _isCompleted = CheckCompletion();
        }
        private void ShowNextSentence()
        {
            ClearText();
            if (_currentSentenceInd == _sentences.Length)
            {
                _isCompleted = true;
                return;
            }

            StartCoroutine(WriteSentence(_sentences[_currentSentenceInd]));
        }
        private void SkipTextAnimation()
        {
            StopAllCoroutines();
            _textBox.text = _sentences[_currentSentenceInd];
            _isTypingNow = false;
            _currentSentenceInd++;
            _isCompleted = CheckCompletion();
        }

        private void ClearText()
        {
            _textBox.text = string.Empty;
        }
        private bool CheckCompletion()
        {
            return _currentSentenceInd == _sentences.Length;
        }
        #endregion

        #region IInteractable realisation
        public GameObject InteractableObject { get { return this.gameObject; } }
        public void Interact()
        {
            if (!_isDisplaying)
                ShowDialogue();
            else if (_isTypingNow)
                SkipTextAnimation();
            else if (!_isCompleted)
                ShowNextSentence();
            else
                FinishDialogue();

            ShowInteractionHint();
        }

        public void ShowInteractionHint()
        {
            if (!_hintDisplay.activeSelf)
                _hintDisplay.SetActive(true);
            if (!_isDisplaying) _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startDialogueHint);
            else if (!_isCompleted) _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_continueDialogueHint);

        }
        public void HideInteractionHint()
        {
            _hintDisplay.GetComponent<HintDisplay>().HideHint();
        }

        #endregion

        #region IPlayerMovementRestrictor implementation
        public void DisablePlayerMovement()
        {
            _playerInput.IsMovementEnabled = false;
        }

        public void EnablePlayerMovement()
        {
            _playerInput.IsMovementEnabled = true;
        }
        #endregion
    }
}
