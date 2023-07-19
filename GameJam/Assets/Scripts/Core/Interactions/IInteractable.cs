using UnityEngine;

namespace GameJam.Core.Interactions
{
    //Interface indicates, that interaction with object is possible
    public interface IInteractable
    {
        //Store gameobject to interact with
        public GameObject InteractableObject { get; }

        //Method to interact with object
        public void Interact(GameObject sender);

        //Methods to show interaction hint, when interaction is possible
        public void ShowInteractionHint(GameObject sender);
        public void HideInteractionHint();
    }
}
