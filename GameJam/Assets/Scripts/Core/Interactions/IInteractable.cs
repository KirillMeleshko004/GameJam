using UnityEngine;

namespace GameJam.Core.Interactions
{
    //Interface indicates, that interaction with object is possible
    public interface IInteractable
    {
        //Keep gameobject to interact with
        public GameObject InteractableObject { get; }

        //Method to interact with object
        public void Interact();

        //Methods to show interaction hint, when interaction is possible
        public void ShowInteractionHint();
        public void HideInteractionHint();
    }
}
