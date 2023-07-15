using GameJam.Core.Interactions;
using ScriptableObjects.Readables;
using UnityEngine;

namespace GameJam.Items
{
    public class PC : BaseInteractable
    {
        #region Local const
        private const string SHOW_HINT = "Press E to show text";
        private const string HIDE_HINT = "Press E to hide text";
        #endregion

        #region Variables
        [SerializeField]
        private Readable _readable;
        [SerializeField]
        protected GameObject _pcDisplay;

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
        }
        private void HideReadable()
        {
        }
        #endregion

        #region BaseInteractable implementation
        public override GameObject InteractableObject {  get { return _interactableObject; } }

        public override void HideInteractionHint()
        {
            throw new System.NotImplementedException();
        }

        public override void Interact()
        {
            throw new System.NotImplementedException();
        }

        public override void ShowInteractionHint()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateHint()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
