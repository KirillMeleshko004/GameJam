using GameJam.Core.Interactions;
using GameJam.Inputs;
using ScriptableObjects.Readables;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameJam.Items
{
    public class Sofa : BaseInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private Animator _playerAnim;
        [Header("Bool trigger name to play different sittin animation")]
        [SerializeField]
        private string _animatorTriggerBoolName = "isSittingOnSofa";


        private GameObject _interactableObject;
        private bool _isSitting = false;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _sitDownHint = "Prees E to sit down";
        [SerializeField]
        private string _standUpHint = "Prees E to stand up";
        #endregion

        #region Properties
        #endregion

        #region Built-in methods
        private void Start()
        {
            _interactableObject = this.gameObject;
        }
        #endregion

        #region Custom methods

        private void SitDown()
        {
            _playerAnim.SetBool(_animatorTriggerBoolName, true);
            UpdateHint();
        }
        private void StandUp()
        {
            _playerAnim.SetBool(_animatorTriggerBoolName, false);
            UpdateHint();
        }
        protected void UpdateHint()
        {
            if (!_isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return _interactableObject; } }
        public override void Interact()
        {
            if (!_isSitting)
            {
                DisablePlayerMovement();
                SitDown();
            }
            else
            {
                EnablePlayerMovement();
                StandUp();
            }

            _isSitting = !_isSitting;

            UpdateHint();
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (!_isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
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
