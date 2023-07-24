using GameJam.Core.Dialogue;
using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Items
{
    internal class Table : BaseInteractable, ISerializationCallbackReceiver
    {
        #region Variables
        [SerializeField]
        private GameObject _player;

        [SerializeField]
        private List<InteractionsInfo> _interactionsInfoHelper = new();

        private readonly Dictionary<GameObject, InteractionsInfo> _interactionsInfo = new();

        [SerializeField]
        private DialogueScript _dialogueScript;
        [SerializeField]
        private TextAsset _dialogueText;

        [Header("If disposable - can handle only one interaction cycle")]
        [SerializeField]
        private bool isDisposable = false;


        [Header("Hint messages")]
        [SerializeField]
        private string _startDialogueHint = "Press E to talk to Derek and Anna";
        [SerializeField]
        private string _startInteractionHint = "Press E to Interact";
        [SerializeField]
        private string _stopInteractionHint = "Press E to leave the table";
        [SerializeField]
        private string _continueInteractionHint = "Press E to continue";
        #endregion


        #region Helper classes
        [Serializable]
        private sealed class InteractionsInfo
        {
            [Header("Interacting object info")]
            public GameObject target = null;
            public PersonObject personObject = null;

            [Header("Offset from table center")]
            public Vector3 targetOffset;

            [Header("Defines object rotation")]
            public bool isFaceRight;

            [Header("If null interaction would be performed without cup")]
            public GameObject cup = null;

            [Header("Start state of object")]
            public bool isInteracting;

            public bool canStopInteraction = false;

            [NonSerialized]
            public bool isDrinkingNow = false;

            [Header("Fires while interaction starts")]
            public UnityEvent onInteractionStarts;
            [Header("Fires while interaction ends")]
            public UnityEvent onInteractionEnds;

        }
        #endregion

        #region Built in methods
        private void Start()
        {
            _dialogueScript.OnDialogueSuspensionChanged += DialogueSuspensionStateChanged;
        }
        #endregion

        #region Custom methods

        private void DialogueSuspensionStateChanged(bool state)
        {
            if(state)
            {
                foreach (var kvp in _interactionsInfo)
                    kvp.Value.personObject.DisableInteractions();
            }
            else
            {
                foreach (var kvp in _interactionsInfo)
                {
                    if(_dialogueScript.IsDialogueComleted) kvp.Value.canStopInteraction = true;
                    kvp.Value.personObject.EnableInteractions();
                }
            }
        }

        private IEnumerator StartInteraction(InteractionsInfo interaction)
        {
            interaction.personObject.DisableInteractions();
            SetInteraction(interaction, true);

            MoveToCorrectPosition(interaction.target, interaction.targetOffset);
            while (!Mover.IsAtTarget(interaction.target))
                yield return new WaitForFixedUpdate();

            SetCorrectRotation(interaction.target, interaction.isFaceRight, interaction.cup != null);

            if(interaction.cup != null) StartCoroutine(TakeCup(interaction));
            else interaction.personObject.EnableInteractions();
        }
        private IEnumerator StopInteraction(InteractionsInfo interaction)
        {
            interaction.personObject.DisableInteractions();

            if (isDisposable) DisableTable();

            if (interaction.cup != null)
            {
                while (interaction.isDrinkingNow)
                    yield return new WaitForFixedUpdate();
                StartCoroutine(PutCupBack(interaction));
            }
            else
            {
                SetInteraction(interaction, false);
                interaction.personObject.EnableInteractions();
            }
        }
        private void ContinueInteraction(InteractionsInfo interaction)
        {
            if (_dialogueScript.IsDialogueComleted)
            {
                StartCoroutine(StopInteraction(interaction));
                _dialogueScript.FinishDialogue();
            }

            else if (!_dialogueScript.IsStarted)
            {
                _dialogueScript.StartDialogue(_dialogueText);
                UpdateInteractionHint();
            }

            else _dialogueScript.ShowNextSentence();
        }


        private void SetInteraction(InteractionsInfo interaction, bool isInteracting)
        {
            interaction.isInteracting = isInteracting;
            if (isInteracting)
            {
                interaction.onInteractionStarts?.Invoke();
                interaction.personObject.DisableMovements();
            }
            else
            {
                interaction.personObject.EnableMovements();
                interaction.onInteractionEnds?.Invoke();
            }
        }
        private IEnumerator TakeCup(InteractionsInfo interaction)
        {
            PersonObject personObject = interaction.personObject;
            
            AnimInfo animInfo = personObject.AnimInfo.GetAnimationInfo(AnimationTypes.TakeCoffee);

            interaction.cup.SetActive(false);

            personObject.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);

            interaction.personObject.EnableInteractions();
        }
        private IEnumerator PutCupBack(InteractionsInfo interaction)
        {
            PersonObject personObject = interaction.personObject;

            AnimInfo animInfo = personObject.AnimInfo.GetAnimationInfo(AnimationTypes.LeaveTable);

            personObject.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);

            interaction.cup.SetActive(true);
            
            SetInteraction(interaction, false);
            interaction.personObject.EnableInteractions();
        }
        private IEnumerator Drink(InteractionsInfo interaction)
        {
            interaction.isDrinkingNow = true;

            AnimInfo animInfo = interaction.personObject.AnimInfo.GetAnimationInfo(AnimationTypes.DrinkCoffee);

            interaction.personObject.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            interaction.isDrinkingNow = false;
        }
        private void MoveToCorrectPosition(GameObject gameObject, Vector3 offset)
        {
            Vector3 targerPos = transform.position + offset;
            Mover.AddMovement(gameObject, targerPos);
        }
        private void SetCorrectRotation(GameObject gameObject, bool isFaceRight, bool isWithCup)
        {
            Vector2 scale = gameObject.transform.localScale;
            float newXScale = isFaceRight && isWithCup || !isFaceRight && !isWithCup ? 
                Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);

            Vector2 newScale = new Vector2(newXScale, scale.y);

            gameObject.transform.localScale = newScale;

            //positive without cup - left
            //positive with cup  - right
        }
        private void UpdateInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            HintDisplay display = _hintDisplay.GetComponent<HintDisplay>();

            if (_dialogueText != null && !_dialogueScript.IsStarted) display.DisplayHint(_startDialogueHint);
            else display.DisplayHint(_continueInteractionHint);
        }
        private void DisableTable()
        {
            GetComponentInChildren<BoxCollider2D>().enabled = false;
        }

        #region Public methods
        public void Drink(GameObject drinker)
        {
            StartCoroutine(Drink(_interactionsInfo[drinker]));
        }
        #endregion

        #endregion


        #region BaseInteractable implementation
        public override GameObject InteractableObject { get { return gameObject; } }

        public override void Interact(GameObject sender)
        {
            if (!_interactionsInfo.ContainsKey(sender)) return;

            InteractionsInfo interaction = _interactionsInfo[sender];

            if (!interaction.isInteracting) StartCoroutine(StartInteraction(interaction));

            else if (interaction.canStopInteraction) StartCoroutine(StopInteraction(interaction));

            else ContinueInteraction(interaction);
        }

        public override void ShowInteractionHint()
        {
            if (_player == null) return;
            InteractionsInfo interaction = _interactionsInfo[_player];

            base._hintDisplay.SetActive(true);
            HintDisplay display = _hintDisplay.GetComponent<HintDisplay>();

            if (!interaction.isInteracting) display.DisplayHint(_startInteractionHint);
            else if (interaction.canStopInteraction) 
                display.DisplayHint(_stopInteractionHint);
            else if(_dialogueText != null && !_dialogueScript.IsStarted) display.DisplayHint(_startDialogueHint);
            else display.DisplayHint(_continueInteractionHint);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }
        #endregion

        #region ISerializationCallbackReceiver
        public void OnBeforeSerialize()
        {
            //No need
        }

        public void OnAfterDeserialize()
        {
            _interactionsInfo.Clear();
            foreach(InteractionsInfo info in _interactionsInfoHelper)
                _interactionsInfo.TryAdd(info.target, info);
        }
        #endregion
    }
}
