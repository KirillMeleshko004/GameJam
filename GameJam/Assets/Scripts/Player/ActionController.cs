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
        private readonly Queue<IInteractable> _interactables = new();

        private bool _areInteractionsDisabled = false;
        private bool _prevState = false;
        #endregion

        #region Properties
        public bool AreInteractionsDisabled { get { return _areInteractionsDisabled; }
            set
            {
                if (_prevState != value)
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
                IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();

                if (AreInteractionsDisabled)
                {
                    interactable.HideInteractionHint();
                    return;
                }

                _interactables.Enqueue(interactable);
                interactable.ShowInteractionHint(gameObject);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                if (AreInteractionsDisabled == _prevState) return;
                _prevState = AreInteractionsDisabled;

                IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();
                if (AreInteractionsDisabled)
                    interactable.HideInteractionHint();
                else
                    interactable.ShowInteractionHint(gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
                if (AreInteractionsDisabled) return;
                IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();
                if(_interactables.Count != 0)
                    _interactables.Dequeue();
                interactable.HideInteractionHint();
            }
        }
        #endregion


        #region Custom methods
        private void HandleInteractions()
        {
            if (AreInteractionsDisabled) return;
            if (_interactables.Count == 0) return;
            if (_playerInput.BasicInteractionInput)
            {
                IInteractable interactable = _interactables.Peek();
                interactable.Interact(gameObject);
                //GameObject.Destroy(interactable.InteractableObject);
            }
        }
        #endregion
    }
}
