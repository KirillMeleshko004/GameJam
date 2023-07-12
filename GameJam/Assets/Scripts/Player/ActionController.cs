using GameJam.Core;
using GameJam.Inputs;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionController : MonoBehaviour
    {
        #region Variables
        private PlayerInput _playerInput;
        private readonly Queue<IInteractable> _interactables = new();
        #endregion

        #region Properties

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
                _interactables.Enqueue(interactable);
                interactable.ShowInteractionHint();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.root.CompareTag("Interactable"))
            {
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
            if (_interactables.Count == 0) return;
            if (_playerInput.BasicInteractionInput)
            {
                IInteractable interactable = _interactables.Peek();
                interactable.Interact();
                //GameObject.Destroy(interactable.InteractableObject);
            }
        }
        #endregion
    }
}
