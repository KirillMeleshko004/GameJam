using GameJam.Core.Interactions;
using GameJam.Inputs;
using ScriptableObjects.Readables;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable item
    public class Item : BaseInteractable, IPlayerMovementRestrictor
    {

        #region Variables
        [Header("Readable object to show")]
        [SerializeField]
        private Readable _readable;
        [Header("Display, where text is showing")]
        [SerializeField]
        protected GameObject _readableDisplay;


        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        private bool _isShowing = false;


        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _showReadableHint = "Press E to show text";
        [SerializeField]
        private string _hideReadableHint = "Press E to hide text";
        #endregion


        #region Custom methods
        private void ShowReadable()
        {
            _readableDisplay.SetActive(true);
            _readableDisplay.GetComponent<ReadableDisplay>().DisplayReadable(_readable);
        }
        private void HideReadable()
        {
            _readableDisplay.GetComponent<ReadableDisplay>().HideReadable();
            _readableDisplay.SetActive(false);
        }
        protected void UpdateHint()
        {
            if (!_isShowing)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_showReadableHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_hideReadableHint);
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact()
        {
            if(!_isShowing)
            {
                DisablePlayerMovement();
                ShowReadable();
            }
            else
            {
                EnablePlayerMovement();
                HideReadable();
            }

            _isShowing = !_isShowing;

            UpdateHint();
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (!_isShowing)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_showReadableHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_hideReadableHint);
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
