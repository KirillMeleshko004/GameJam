using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Core.Dialogue
{
    public class DialogueScript : MonoBehaviour
    {
        #region Variables
        [Header("Display, where text is showing")]
        [SerializeField]
        private GameObject _dialogueWindowPrefab;
        [Header("Delay after printing one character")]
        [SerializeField]
        private float _nextCharDelay = 0.01f;

        [SerializeField]
        private List<SuspensionInfo> _linesToSuspend = new();


        private readonly Dictionary<string, TextMeshPro> _tmrpoForPerson = new();
        private int _currentLineInd = 0;
        private TextMeshPro _currentActiveText = null;
        private readonly List<DialogueLine> _dialogueLines = new();
        private bool _isTyingNow = false;

        private bool _isSuspended = false;
        private bool _isSuspensionRequired = false;
        private SuspensionInfo _currentSuspensionInfo = null;
        #endregion

        #region Properties
        public bool IsStarted { get; private set; } = false;
        public bool IsDialogueComleted { get; private set; } = false;
        public bool IsSuspended { get { return _isSuspended; } 
            private set 
            { 
                _isSuspended = value;
                OnDialogueSuspensionChanged?.Invoke(_isSuspended);
            } 
        }
        #endregion

        #region Events
        public event Action<bool> OnDialogueSuspensionChanged;
        #endregion

        #region Helper classes
        [Serializable]
        private sealed class SuspensionInfo
        {
            [Header("Line after which dialogue would be suspended")]
            public DialogueLine lineToSuspend;
            public float suspensionTime = 0f;
            public UnityEvent onDialogueSuspended = new();
        }
        [Serializable]
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

        #region Custom methods

        #region Public methods
        public void StartDialogue(TextAsset dialogue)
        {
            ParseTextFile(dialogue);
            SetDialogueWindows(_dialogueLines);
            IsStarted = true;
            ShowNextSentence();
        }
        public void FinishDialogue()
        {
            foreach (var kvp in _tmrpoForPerson)
                GameObject.Destroy(kvp.Value.transform.parent.parent.gameObject);
        }
        public void ShowNextSentence()
        {
            StartCoroutine(ShowNextSentecneAsCourotine());
        }
        #endregion
        private IEnumerator ShowNextSentecneAsCourotine()
        {
            if(_isSuspensionRequired)
            {
                StartCoroutine(Suspend(_currentSuspensionInfo));
                while (IsSuspended)
                    yield return new WaitForFixedUpdate();

            }

            if (IsDialogueComleted) yield break;

            _isSuspensionRequired = IsSuspensionAfterCurrentLineRequired();

            if(_isTyingNow)
            {
                SkipTypingAnimation();
                yield break;
            }

            SwitchActiveDisplay();

            StartCoroutine(WriteLine(_dialogueLines[_currentLineInd].Line, _currentActiveText));
        }

        private IEnumerator WriteLine(string line, TextMeshPro tmpro)
        {
            _isTyingNow = true;

            for (int i = 0; i < line.Length; i++)
            {
                tmpro.text = string.Concat(tmpro.text, line[i]);

                yield return new WaitForSeconds(_nextCharDelay);
            }

            _currentLineInd++;
            _isTyingNow = false;

            IsDialogueComleted = IsDialogueCompleted();
        }
        private void SkipTypingAnimation()
        {
            StopAllCoroutines();
            _currentActiveText.text = _dialogueLines[_currentLineInd].Line;
            _isTyingNow = false;
            _currentLineInd++;

            IsDialogueComleted = IsDialogueCompleted();
        }

        private bool IsSuspensionAfterCurrentLineRequired()
        {
            foreach(SuspensionInfo info in _linesToSuspend)
            {
                if(info.lineToSuspend.Line == _dialogueLines[_currentLineInd].Line &&
                    info.lineToSuspend.Person == _dialogueLines[_currentLineInd].Person)
                {
                    _currentSuspensionInfo = info;
                    return true;
                }
            }
            return false;
        }
        private bool IsDialogueCompleted()
        {
            return _currentLineInd == _dialogueLines.Count && !_isSuspensionRequired;
        }
        private IEnumerator Suspend(SuspensionInfo info)
        {
            IsSuspended = true;

            _currentActiveText.transform.parent.parent.gameObject.SetActive(false);

            info.onDialogueSuspended?.Invoke();

            yield return new WaitForSeconds(info.suspensionTime);

            foreach (var kvp in _tmrpoForPerson)
            {
                SetDialogueWindowScale(kvp.Value.transform.parent.parent.gameObject);
            }

            _isSuspensionRequired = false;
            IsDialogueComleted = IsDialogueCompleted();
            IsSuspended = false;
        }

        private void ParseTextFile(TextAsset textFile)
        {
            _dialogueLines.Clear();
            string[] lines = textFile.text.Split('\n');
            foreach (string line in lines)
            {
                if (line.Trim() == string.Empty) continue;
                string[] personLinePair = (from str in line.Split('|').ToList<string>() select str.Trim()).ToArray<string>();
                _dialogueLines.Add(new DialogueLine(personLinePair[0], personLinePair[1]));
            }
        }

        private void SetDialogueWindowActive(bool isActive, TextMeshPro text)
        {
            if (text != null)
            {
                text.transform.parent.parent.gameObject.SetActive(isActive);
            }
        }
        private void SetDialogueWindowScale(GameObject dialogueWindow)
        {
            dialogueWindow.transform.localScale = Vector2.one;

            float xProportion = 1f;
            float yProportion = 1f;

            Transform parentTransform = dialogueWindow.transform;

            while (parentTransform != dialogueWindow.transform.root)
            {
                xProportion /= parentTransform.parent.localScale.x;
                yProportion /= parentTransform.parent.localScale.y;

                parentTransform = parentTransform.parent;
            }

            dialogueWindow.transform.localScale = new Vector2(dialogueWindow.transform.localScale.x * xProportion,
                dialogueWindow.transform.localScale.y * yProportion);
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
                  PersonToObjectAssign.GetObjectToPerson(names[i]).transform);
                dialogueWindow.transform.localPosition = PersonToObjectAssign.GetOffsetForPerson(names[i]);
                SetDialogueWindowScale(dialogueWindow);
                TextMeshPro dialogueTmpro = dialogueWindow.GetComponentInChildren<TextMeshPro>();
                _tmrpoForPerson.TryAdd(names[i], dialogueTmpro);
                SetDialogueWindowActive(false, dialogueTmpro);
            }
        }
        private void ClearText(TextMeshPro text)
        {
            text.text = string.Empty;
        }
        private void SwitchActiveDisplay()
        {
            if (_currentActiveText != null)
                SetDialogueWindowActive(false, _currentActiveText);

            _currentActiveText = _tmrpoForPerson[_dialogueLines[_currentLineInd].Person];
            ClearText(_currentActiveText);
            SetDialogueWindowActive(true, _currentActiveText);
        }
        #endregion
    }
}
