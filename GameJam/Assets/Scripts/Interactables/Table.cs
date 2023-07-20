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
        private GameObject _player;


        [SerializeField]
        private DialogueScript _dialogue;
        [SerializeField]
        private TextAsset _dialogueText = null;

        [SerializeField]
        private Animator _tableAnim;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _takeCup = "Prees E to take cup";
        [SerializeField]
        private string _drink = "Prees E to drink coffee";
        [SerializeField]
        private string _leave = "Prees E to leave the table";

        [SerializeField]
        private float _drinkStartDelay = 2f;

        [Header("Postition offset relative to table to position objects. Can be change by changing property value")]
        [SerializeField]
        private Vector3 _positionOffset = Vector3.zero;
        
        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _takeCoffeeHint = "Press E to take coffee";
        [SerializeField]
        private string _leaveTableHind = "Press E to leave the table";
        [SerializeField]
        private string _continueDialogueHint = "Press E to continue";
        #endregion

        #region Properties
        public Vector3 PositionOffset { get { return _positionOffset; } set { _positionOffset = value; } }
        public TextAsset DiialogueText { get { return _dialogueText; } set { _dialogueText = value; } }
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

            public bool isPlayer;
            public InteractionObjectInfo(PersonObject objectInfo, bool isPlayer)
            {
                this.objectInfo = objectInfo;
                this.isPlayer = isPlayer;
                this.gameObject = objectInfo.gameObject;
            }
        }
        #endregion

        #region Custom methods
        private void SetAnimTrigger(Animator animator, AnimInfo animInfo)
        {
            if (animInfo.TriggerType == TriggerTypes.Bool)
                animator.SetBool(animInfo.TriggerValueName, animInfo.TriggerValue);
            else
                animator.SetTrigger(animInfo.TriggerValueName);
        }

        private void ResetTableFor(InteractionObjectInfo interactionObject)
        {
            interactionObject.isInteracting = false;
            interactionObject.isDrinking = false;
            interactionObject.isLeaving = false;
            interactionObject.canLeave = false;
        }

        private IEnumerator TakeCup(InteractionObjectInfo interactionObject)
        {
            interactionObject.objectInfo.DisableMovements();

            interactionObject.isInteracting = true;
            interactionObject.canLeave = _dialogueText == null;


            Mover.AddMovement(interactionObject.gameObject, new Vector3(transform.position.x,
                interactionObject.gameObject.transform.position.y,
                interactionObject.gameObject.transform.position.z) + _positionOffset);

            while (!Mover.IsAtTarget(interactionObject.gameObject))
                yield return new WaitForFixedUpdate();

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.TakeCoffee);

            interactionObject.objectInfo.DisableInteractions();

            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            interactionObject.objectInfo.EnableInteractions();
            Invoke(nameof(StartDrinkAnimation), _drinkStartDelay);
        }

        //need enum?
        private void StartDrinkAnimation(InteractionObjectInfo interactionObject)
        {
            if (interactionObject.isLeaving)
            {
                CancelInvoke(nameof(StartDrinkAnimation));
                return;
            }
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

            ResetTableFor(interactionObject);

            interactionObject.objectInfo.EnableMovements();
            interactionObject.objectInfo.EnableInteractions();
        }

        private void HandleDialogue()
        {
            if (!_dialogue.IsDisplaying)
            {
                _dialogue.StartDialogue(_dialogueText);
            }
            else if (_dialogue.IsTypingNow)
                _dialogue.SkipTextAnimation();
            else if (!_dialogue.IsCompleted)
                _dialogue.ShowNextSentence();
            else
            {
                _dialogue.FinishDialogue();
                StartCoroutine(LeaveTheTable(_objectsToInteract[_player]));
            }
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact(GameObject sender)
        {
            if (!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(),
                    sender == _player));

            bool startDialogue = _dialogueText != null && _objectsToInteract[sender].isPlayer &&
                !_objectsToInteract[sender].canLeave;

            if (!_objectsToInteract[sender].isInteracting)
                StartCoroutine(TakeCup(_objectsToInteract[sender]));
            else if (startDialogue)
                    HandleDialogue();
            else
                StartCoroutine(LeaveTheTable(_objectsToInteract[sender]));

        }

        public override void ShowInteractionHint(GameObject sender)
        {
            if (sender != _player) return;

            if (!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(), sender == _player));

            base._hintDisplay.SetActive(true);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        #endregion
    }
}
