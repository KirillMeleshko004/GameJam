using GameJam.Core.Interactions;
using GameJam.Inputs;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Player
{
    //Class to handle all actions, except movement
    [RequireComponent(typeof(PlayerInput))]
    public class ActionController : MonoBehaviour
    {
        #region Variables
        private PlayerInput _playerInput;

        private IInteractable _objectToInteract = null;
      
        private bool _areInteractionsDisabled = false;
        #endregion

        #region Built-in methods
        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void LateUpdate()
        {
            if (!_areInteractionsDisabled) HandleInteractions();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                _objectToInteract = collision.transform.root.GetComponentInChildren<IInteractable>();
                _objectToInteract.ShowInteractionHint();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                _objectToInteract.HideInteractionHint();
                _objectToInteract = null;
            }
        }
        #endregion


        #region Custom methods

        private void HandleInteractions()
        {
            if (_objectToInteract != null && _playerInput.BasicInteractionInput)
            {
                _objectToInteract.Interact(gameObject);
            }
        }

        public void DisableInteractions()
        {
            GetComponentInChildren<BoxCollider2D>().enabled = false;
            _areInteractionsDisabled = true;
        }
        public void EnableInteractions()
        {
            GetComponentInChildren<BoxCollider2D>().enabled = true;
            _areInteractionsDisabled = false;
        }
        #endregion
    }
}
