using GameJam.Core.Interactions;
using GameJam.Inputs;
using GameJam.Player;
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
        [SerializeField]
        private string _shootHint = "Prees E to shoot";

        [Header("Length of animation clips")]
        [SerializeField]
        private float _sitDownAnimTime = 0.66f;

        [Header("Is last scene")]
        [SerializeField]
        private bool _isDayX = false;
        #endregion

        #region Properties
        #endregion

        #region Custom methods
        private IEnumerator StandUp()
        {
            StartCoroutine(DisableColliderForTime(_sitDownAnimTime));
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

        private void Shoot()
        {
            DisablePlayerMovement();
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(false);
            Suicide player = _playerAnim.gameObject.GetComponent<Suicide>();
            player.ShootDown();

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
                StartCoroutine(SitDown());
            else if (!_isDayX)
                StartCoroutine(StandUp());
            else
                Shoot();

            _isSitting = !_isSitting;
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (!_isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else if (!_isDayX)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_shootHint);
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
