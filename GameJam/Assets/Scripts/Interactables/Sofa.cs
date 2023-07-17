using GameJam.Core.Interactions;
using GameJam.Inputs;
using System;
using System.Collections;
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

        private bool _isSitting = false;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _sitDownHint = "Prees E to sit down";
        [SerializeField]
        private string _standUpHint = "Prees E to stand up";

        [Header("Length of animation clips")]
        [SerializeField]
        private float _sitDownAnimTime = 0.66f;

        [SerializeField]
        private bool _isOnCooldown = false;
        #endregion

        #region Properties
        #endregion

        #region Custom methods
        private IEnumerator StandUp()
        {
            _playerAnim.SetBool(_animatorTriggerBoolName, false);
            yield return new WaitForSeconds(_sitDownAnimTime);
            EnablePlayerMovement();
        }
        private IEnumerator SitDown()
        {
            StartCoroutine(DisableColliderForTime(_sitDownAnimTime));
            DisablePlayerMovement();
            _playerAnim.SetBool(_animatorTriggerBoolName, true);
            yield return new WaitForSeconds(_sitDownAnimTime);
        }
        protected void UpdateHint()
        {
            if (!_isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
        }

        private IEnumerator DisableColliderForTime(float time)
        {
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(false);
            yield return new WaitForSeconds(time);
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(true);
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact()
        {
            if (!_isSitting)
            {
                StartCoroutine(SitDown());
            }
            else
                StartCoroutine(StandUp());

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
