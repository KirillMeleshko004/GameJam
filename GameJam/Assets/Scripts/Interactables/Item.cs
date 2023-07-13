using GameJam.Core.Interactions;
using ScriptableObjects.Readables;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable item
    public class Item : BaseInteractable
    {
        #region Local const
        private const string SHOW_HINT = "Press E to show text";
        private const string HIDE_HINT = "Press E to hide text";
        #endregion

        #region Variables
        [SerializeField]
        private Readable _readable;
        [SerializeField]
        protected GameObject _readableDisplay;

        private GameObject _interactableObject;

        private bool _isShowing = false;
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
        protected override void UpdateHint()
        {
            if (!_isShowing)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(SHOW_HINT);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(HIDE_HINT);
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return _interactableObject; } }
        public override void Interact()
        {
            Debug.Log("Interaction");
            if(!_isShowing)
                ShowReadable();
            else
                HideReadable();

            _isShowing = !_isShowing;

            UpdateHint();
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (!_isShowing)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(SHOW_HINT);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(HIDE_HINT);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }
        #endregion
    }
}
