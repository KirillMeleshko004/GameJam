using GameJam.Core;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable item
    public class Item : MonoBehaviour, IInteractable
    {
        #region Variables
        #endregion

        public GameObject InteractableObject { get; private set; }

        #region Built-in methods
        private void Start()
        {
            InteractableObject = this.gameObject;
        }
        #endregion




        #region IInteractable realisation
        public void Interact()
        {
            Debug.Log("Interaction");
        }

        public void ShowInteractionHint()
        {
            Debug.Log("Show hint");
        }
        public void HideInteractionHint()
        {
            Debug.Log("Hide hint");
        }
        #endregion
    }
}
