using UnityEngine;

namespace GameJam.Core.Interactions
{
    //Base class for interactable objects
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {

        [Header("Display, where interaction hint is showing")]
        [SerializeField]
        protected GameObject _hintDisplay;

        public abstract GameObject InteractableObject { get; }

        public abstract void HideInteractionHint();

        public abstract void Interact(GameObject sender);

        public abstract void ShowInteractionHint(GameObject sender);

    }
}
