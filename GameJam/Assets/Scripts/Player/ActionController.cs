using GameJam.Core.Interactions;
using GameJam.Inputs;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameJam.Player
{
    //Class to handle all actions, except movement
    [RequireComponent(typeof(PlayerInput))]
    public class ActionController : MonoBehaviour
    {
        #region Variables
        private PlayerInput _playerInput;

        private Queue<IInteractable> _interactables = new();
      

        private bool _areInteractionsDisabled = false;
        private bool _prevState = false;
        #endregion

        #region Properties
        public bool AreInteractionsDisabled { get { return _areInteractionsDisabled; }
            set
            {
                gameObject.GetComponent<Rigidbody2D>().WakeUp();
                _areInteractionsDisabled = value;
            }
        }
        #endregion


        #region Built-in methods
        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void LateUpdate()
        {
            HandleInteractions();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                if (AreInteractionsDisabled) return;

                AddNewInteractable(collision.transform.root.GetComponent<IInteractable>());     
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (AreInteractionsDisabled == _prevState) return;
            _prevState = AreInteractionsDisabled;

            if (collision.transform.root.CompareTag("Interactable"))
            {
                IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();

                //Object is in trigger zone
                //If diasabling during trigger zone hide hint and remove interactable
                //if enabling - add interactable and show hint
                if (AreInteractionsDisabled)
                    DeleteInteractable(interactable);
                else
                    AddNewInteractable(interactable);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                DeleteInteractable(collision.transform.root.GetComponent<IInteractable>());
            }
        }
        #endregion


        #region Custom methods

        private void AddNewInteractable(IInteractable interactable)
        {
            if (_interactables.Count == 0)
                interactable?.ShowInteractionHint();

            if (!_interactables.Contains(interactable))
                _interactables.Enqueue(interactable);
        }
        private void DeleteInteractable(IInteractable interactable)
        {
            if (_interactables.Contains(interactable))
                _interactables.Dequeue();

            interactable?.HideInteractionHint();

            _interactables.TryPeek(out IInteractable top);
            top?.ShowInteractionHint();
        }

        private void HandleInteractions()
        {
            if (AreInteractionsDisabled) return;
            if (_interactables.Count == 0) return;
            if (_playerInput.BasicInteractionInput)
            {
                _interactables.TryPeek(out IInteractable top);
                top?.Interact(gameObject);
            }
        }
        #endregion
    }
}
