using GameJam.Core.Dialogue;
using GameJam.Core.Interactions;
using GameJam.Inputs;
using GameJam.ScriptableObjects.Dialogue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameJam.Items
{
    public class DialogueScript : MonoBehaviour, IPlayerMovementRestrictor
    {
        #region Variables
        [SerializeField]
        private PersonToObjectAssign _pto;

        [SerializeField]
        private float _nextCharDelay = 0.01f;

        [Header("Display, where text is showing")]
        [SerializeField]
        private GameObject _dialogueWindowPrefab;

        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        private readonly Dictionary<string, TextMeshPro> _tmrpoForPerson = new Dictionary<string, TextMeshPro>();
        private int _currentLineInd = 0;
        private TextMeshPro _currentActiveText = null;

        private List<DialogueLine> DialogueLines { get; } = new();
        #endregion

        #region Properties
        public bool IsCompleted { get; private set; } = false;
        public bool IsDisplaying { get; private set; } = false;
        public bool IsTypingNow { get; private set; } = false;

        public bool IsSuspended { get; private set; } = false;

        public string CurrentLine { get; private set; }
        #endregion

        #region Built-in methods
        #endregion


        #region Custom methods
        public void StartDialogue(TextAsset text)
        {
            IsDisplaying = true;

            ParseTextFile(text);

            SetDialogueWindows(DialogueLines);
            
            ShowNextSentence();
        }

        public void FinishDialogue()
        {
            foreach(var kvp in _tmrpoForPerson)
                GameObject.Destroy(kvp.Value.transform.parent.parent.gameObject);
            ResetParameters();
        }

        private void ResetParameters()
        {
            IsTypingNow = false;
            IsDisplaying = false;
            _currentLineInd = 0;
            _currentActiveText = null;
            _tmrpoForPerson.Clear();
            DialogueLines.Clear();
        }
        private void SetDialogueWindows(List<DialogueLine> lines)
        {
            List<string> names = new();
            names.AddRange(from DialogueLine line in lines
                           where !names.Contains(line.Person)
                           select line.Person);

            for (int i = 0; i < names.Count; i++)
            {
                GameObject dialogueWindow = Instantiate(_dialogueWindowPrefab, Vector3.zero, Quaternion.identity,
                  _pto.GetObjectToPerson(names[i]).transform);
                dialogueWindow.transform.localPosition = _pto.GetOffsetForPerson(names[i]);
                SetDialogueWindowScale(dialogueWindow);
                TextMeshPro dialogueTmpro = dialogueWindow.GetComponentInChildren<TextMeshPro>();
                _tmrpoForPerson.TryAdd(names[i], dialogueTmpro);
                SetDialogueWindowActive(false, dialogueTmpro);
            }
        }
        private void SetDialogueWindowScale(GameObject dialogueWindow)
        {
            float xProportion = 1f / dialogueWindow.transform.parent.localScale.x;
            float yProportion = 1f / dialogueWindow.transform.parent.localScale.y;
            float zProportion = 1f / dialogueWindow.transform.parent.localScale.z;
            
            dialogueWindow.transform.localScale = new Vector3(dialogueWindow.transform.localScale.x * xProportion,
                dialogueWindow.transform.localScale.y * yProportion, dialogueWindow.transform.localScale.z * zProportion);
        }

        public void ShowNextSentence()
        {
            if(IsSuspended) return;
            if (_currentLineInd == DialogueLines.Count)
            {
                IsCompleted = true;
                return;
            }
            if(_currentActiveText == null)
            {
                _currentActiveText = _tmrpoForPerson[DialogueLines[_currentLineInd].Person];
                ClearText(_currentActiveText);
                SetDialogueWindowActive(true, _currentActiveText);
            }
            else
            {

                SetDialogueWindowActive(false, _currentActiveText);
                _currentActiveText = _tmrpoForPerson[DialogueLines[_currentLineInd].Person];
                ClearText(_currentActiveText);
                SetDialogueWindowActive(true, _currentActiveText);
            }

            StartCoroutine(WriteLine(DialogueLines[_currentLineInd].Line, _currentActiveText));
        }

        private IEnumerator WriteLine(string line, TextMeshPro tmpro)
        {
            CurrentLine = line;
            IsTypingNow = true;

            for (int i = 0; i < line.Length; i++)
            {
                tmpro.text = string.Concat(tmpro.text, line[i]);

                yield return new WaitForSeconds(_nextCharDelay);
            }

            IsTypingNow = false;
            _currentLineInd++;
            IsCompleted = CheckCompletion();
        }

        public void SuspendDialogue()
        {
            IsSuspended = true;
        }
        public void SuspendDialogue(float time)
        {
            StartCoroutine(SuspenOnTime(time));
        }
        private IEnumerator SuspenOnTime(float time)
        {
            IsSuspended = true;
            yield return new WaitForSeconds(time);
            IsSuspended = false;
        }
        public void SkipTextAnimation()
        {
            StopAllCoroutines();
            _currentActiveText.text = DialogueLines[_currentLineInd].Line;
            IsTypingNow = false;
            _currentLineInd++;
            IsCompleted = CheckCompletion();
        }
        private bool CheckCompletion()
        {
            return _currentLineInd == DialogueLines.Count;
        }
        private void ClearText(TextMeshPro text)
        {
            text.text = string.Empty;
        }

        private void SetDialogueWindowActive(bool isActive, TextMeshPro text)
        {
            if (text != null)
            {
                text.transform.parent.parent.gameObject.SetActive(isActive);
            }
        }

        private void ParseTextFile(TextAsset textFile)
        {
            DialogueLines.Clear();
            string[] lines = textFile.text.Split('\n');
            foreach (string line in lines)
            {
                string[] personLinePair = (from str in line.Split('|').ToList<string>() select str.Trim()).ToArray<string>();
                DialogueLines.Add(new DialogueLine(personLinePair[0], personLinePair[1]));
            }
        }
        private sealed class DialogueLine
        {
            [SerializeField]
            private string _person;
            [SerializeField]
            private string _line;

            public string Person { get { return _person; } private set { _person = value; } }
            public string Line { get { return _line; } private set { _line = value; } }

            public DialogueLine(string person, string line)
            {
                Person = person;
                Line = line;
            }
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

