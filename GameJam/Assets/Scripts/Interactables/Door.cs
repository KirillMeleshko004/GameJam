using GameJam.Core.Interactions;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable door
    public class Door : BaseInteractable
    {
        #region Local const
        private const string OPEN_HINT = "Press E to open the door";
        private const string CLOSE_HINT = "Press E to close the door";
        #endregion

        #region Variables
        private bool _isClosed = true;
        private GameObject _interactableObject;
        #endregion


        #region Built-in methods
        private void Start()
        {
            _interactableObject = this.gameObject;
        }
        #endregion

        #region Custom methods

        protected override void UpdateHint()
        {
            if (_isClosed)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(OPEN_HINT);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(CLOSE_HINT);
        }
        #endregion


        #region IInteractable realisation
        public override GameObject InteractableObject { get { return _interactableObject; } }
        public override void Interact()
        {
            _isClosed = !_isClosed;
            Debug.Log("Is closed: " + _isClosed.ToString());

            GameObject collider = transform.GetChild(0).gameObject;
            collider.SetActive(_isClosed);

            ShowInteractionHint();
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (_isClosed)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(OPEN_HINT);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(CLOSE_HINT);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            _hintDisplay.SetActive(false);
        }
        #endregion
    }
}
