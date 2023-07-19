using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Inputs;
using GameJam.Player;
using System.Collections;
using UnityEngine;

namespace GameJam.Items
{
    public class Table : BaseInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private DialogueScript _dialogue;
        [SerializeField]
        private TextAsset _dialogueText;

        [SerializeField]
        private Animator _tableAnim;

        [SerializeField]
        private Animator _playerAnim;

        [Header("Bool trigger name to drink animation")]
        [SerializeField]
        private string _animatorTakeCoffeeTriggerName = "takeCoffee";
        [SerializeField]
        private string _animatorDrinkTriggerName = "drinkCoffee";
        [SerializeField]
        private string _animatorLeaveTableTriggerName = "leaveTable";

        private bool _isInteracting = false;
        private bool _isDrinking = false;
        private bool _isLeaving = false;

        private bool _canLeave = false;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _takeCup = "Prees E to take cup";
        [SerializeField]
        private string _drink = "Prees E to drink coffee";

        [Header("Length of animation clips")]
        [SerializeField]
        private float _takingCupAnimTime = 0.66f;
        [SerializeField]
        private float _drinkAnimTime = 0.66f;



        [SerializeField]
        private float _drinkRepeatTime = 10f;
        [SerializeField]
        private float _drinkStartDelay = 2f;


        [SerializeField]
        private Vector3 _playerPosOffset = Vector3.zero;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _takeCoffeeHint = "Press E to take coffee";
        [SerializeField]
        private string _leaveTableHind = "Press E to leave the table";
        [SerializeField]
        private string _continueDialogueHint = "Press E to continue";
        #endregion

        #region Custom methods

        private void ResetTable()
        {
            _isInteracting = false;
            _isDrinking = false;
            _isLeaving = false;
            _canLeave = false;
        }

        private IEnumerator TakeCup()
        {
            Debug.Log("Here");
            DisablePlayerMovement();
            _isInteracting = true;
            _canLeave = _dialogue == null;
            Mover.AddMovement(_playerInput.gameObject, new Vector3(transform.position.x,
                _playerInput.transform.position.y, _playerInput.transform.position.z) + _playerPosOffset);
            while (!Mover.IsAtTarget(_playerInput.gameObject))
                yield return new WaitForFixedUpdate();

            StartCoroutine(DisableColliderForTime(_takingCupAnimTime));
            _playerAnim.SetTrigger(_animatorTakeCoffeeTriggerName);
            yield return new WaitForSeconds(_takingCupAnimTime);

            Invoke(nameof(StartDrinkAnimation), _drinkStartDelay);
        }
        

        private void StartDrinkAnimation()
        {
            if (_isLeaving)
            {
                CancelInvoke(nameof(StartDrinkAnimation));
                return;
            }
            StartCoroutine(Drink());
        }

        private IEnumerator Drink()
        {
            _isDrinking = true;
            _playerAnim.SetTrigger(_animatorDrinkTriggerName);
            yield return new WaitForSeconds(_drinkAnimTime);
            _isDrinking = false;
        }

        private IEnumerator LeaveTheTable()
        {
            _isLeaving = true;
            SetColliderState(false);
            while (_isDrinking) yield return new WaitForFixedUpdate();
            _playerAnim.SetTrigger(_animatorLeaveTableTriggerName);

            yield return new WaitForSeconds(_takingCupAnimTime);
            EnablePlayerMovement();
            ResetTable();
            SetColliderState(true);
        }

        private IEnumerator DisableColliderForTime(float time)
        {
            SetColliderState(false);
            yield return new WaitForSeconds(time);
            SetColliderState(true);
        }
        private void SetColliderState(bool state)
        {
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(state);
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
                StartCoroutine(LeaveTheTable());
            }
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact()
        {
            if (!_isInteracting)
                StartCoroutine(TakeCup());
            else if (!_canLeave)
            {
                if (_dialogue != null)
                {
                    HandleDialogue();
                }
            }
            else
                StartCoroutine(LeaveTheTable());

        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
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
