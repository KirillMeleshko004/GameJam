using UnityEngine;

namespace GameJam.Core.Interactions
{
    //Base class for interactable objects
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {

        [SerializeField]
        protected GameObject _hintDisplay;
        protected abstract void UpdateHint();

        public abstract GameObject InteractableObject { get; }

        public abstract void HideInteractionHint();

        public abstract void Interact();

        public abstract void ShowInteractionHint();

    }
}
