using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Items
{
    public class Table : BaseInteractable
    {
        #region Variables
        [Header("Player gameobject")]
        [SerializeField]
        protected GameObject _player;

        [SerializeField]
        private GameObject _cup;

        [SerializeField]
        private bool _autoDrinkEnabled = true;


        [SerializeField]
        private DialogueScript _dialogue;
        [SerializeField]
        private TextAsset _dialogueText = null;

        [SerializeField]
        private Animator _tableAnim;

        [SerializeField]
        private float _drinkDelay = 2f;

        [Header("Postition offset relative to table to position objects. Can be change by changing property value")]
        [SerializeField]
        private Vector3 _positionOffset = Vector3.zero;
        
        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _takeCoffeeHint = "Press E to take coffee";
        [SerializeField]
        private string _leaveTableHint = "Press E to leave the table";
        [SerializeField]
        private string _startDialogueHint = "Press E to start dialogue";
        [SerializeField]
        private string _continueDialogueHint = "Press E to continue dialogue";
        #endregion

        #region Properties
        public Vector3 PositionOffset { get { return _positionOffset; } set { _positionOffset = value; } }
        public TextAsset DialogueText { get { return _dialogueText; } set { _dialogueText = value; } }
        #endregion

        #region Interactions info
        private readonly Dictionary<GameObject, InteractionObjectInfo> _objectsToInteract = new();
        private sealed class InteractionObjectInfo
        {
            public PersonObject objectInfo;

            public GameObject gameObject;

            public bool isInteracting = false;
            public bool isDrinking = false;
            public bool isLeaving = false;
            public bool canLeave = false;

            public InteractionObjectInfo(PersonObject objectInfo, bool canLeave)
            {
                this.objectInfo = objectInfo;
                this.gameObject = objectInfo.gameObject;
                this.canLeave = canLeave;
            }
        }
        #endregion

        #region Custom methods
        private void ResetTableFor(InteractionObjectInfo interactionObject)
        {
            if (_objectsToInteract.ContainsValue(interactionObject))
                _objectsToInteract.Remove(interactionObject.gameObject);
        }

        private IEnumerator TakeCup(InteractionObjectInfo interactionObject)
        {
            interactionObject.objectInfo.DisableInteractions();
            interactionObject.objectInfo.DisableMovements();

            interactionObject.isInteracting = true;


            Mover.AddMovement(interactionObject.gameObject, new Vector3(transform.position.x,
                interactionObject.gameObject.transform.position.y,
                interactionObject.gameObject.transform.position.z) + _positionOffset);

            while (!Mover.IsAtTarget(interactionObject.gameObject))
                yield return new WaitForFixedUpdate();

            if(interactionObject.gameObject.transform.localScale.x > 0f)
                interactionObject.gameObject.transform.localScale = new Vector3(
                    -interactionObject.gameObject.transform.localScale.x,
                    interactionObject.gameObject.transform.localScale.y,
                    interactionObject.gameObject.transform.localScale.z
                    );

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.TakeCoffee);

            interactionObject.objectInfo.DisableInteractions();

            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            _cup.SetActive(false);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            interactionObject.objectInfo.EnableInteractions();

            if(_autoDrinkEnabled)
                StartCoroutine(StartDrinkAnimation(interactionObject));
        }

        public void Drink(GameObject drinker)
        {
            if (!_objectsToInteract.ContainsKey(drinker))
            {
                _objectsToInteract.Add(drinker, new InteractionObjectInfo(
                            drinker.GetComponent<PersonObject>(),
                            drinker != _player || _dialogueText == null));
            }
            StartCoroutine(StartDrinkAnimation(_objectsToInteract[drinker]));
        }

        private IEnumerator StartDrinkAnimation(InteractionObjectInfo interactionObject)
        {
            yield return new WaitForSeconds(_drinkDelay);

            if (!interactionObject.isLeaving)
                StartCoroutine(Drink(interactionObject));
        }

        private IEnumerator Drink(InteractionObjectInfo interactionObject)
        {
            interactionObject.isDrinking = true;

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.DrinkCoffee);
            interactionObject.objectInfo.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);
            interactionObject.isDrinking = false;
        }

        private IEnumerator LeaveTheTable(InteractionObjectInfo interactionObject)
        {
            interactionObject.isLeaving = true;
            interactionObject.objectInfo.DisableInteractions();

            while (interactionObject.isDrinking) yield return new WaitForFixedUpdate();

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.LeaveTable);
            interactionObject.objectInfo.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);
            _cup.SetActive(true);

            ResetTableFor(interactionObject);

            interactionObject.objectInfo.EnableMovements();
            interactionObject.objectInfo.EnableInteractions();
        }

        private void HandleDialogue()
        {
            if(_dialogue.IsCompleted)
            {
                _dialogue.FinishDialogue();
                _objectsToInteract[_player].canLeave = true;
            }
            else if (!_dialogue.IsDisplaying)
            {
                _dialogue.StartDialogue(_dialogueText);
            }
            else
            {
                _dialogue.ShowNextSentence();   
            }
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact(GameObject sender)
        {
            if (!_objectsToInteract.ContainsKey(sender))
            {
                _objectsToInteract.Add(sender, new InteractionObjectInfo(
                            sender.GetComponent<PersonObject>(),
                            sender != _player || _dialogueText == null));
            }

            if (!_objectsToInteract[sender].isInteracting)
                StartCoroutine(TakeCup(_objectsToInteract[sender]));
            else if (_objectsToInteract[sender].canLeave)
                StartCoroutine(LeaveTheTable(_objectsToInteract[sender]));
            else
                HandleDialogue();

            ShowInteractionHint();
        }

        public override void ShowInteractionHint()
        {
            if (_player == null) return;

            if (!_objectsToInteract.ContainsKey(_player))
                _objectsToInteract.Add(_player, new InteractionObjectInfo(_player.GetComponent<PersonObject>(),
                     _dialogueText == null));

            base._hintDisplay.SetActive(true);
            if (!_objectsToInteract[_player].isInteracting)
            {
                _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_takeCoffeeHint);
            }
            else if(_objectsToInteract[_player].canLeave)
            {
                _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_leaveTableHint);
            }
            else if (!_dialogue.IsDisplaying)
            {
                _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startDialogueHint);
            }
            else
                _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_continueDialogueHint);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        #endregion
    }
}
