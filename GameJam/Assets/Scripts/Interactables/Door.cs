using GameJam.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable door
    public class Door : MonoBehaviour, IInteractable
    {
        #region Variables
        private bool _isClosed = true;
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
            _isClosed = !_isClosed;
            Debug.Log("Is closed: " + _isClosed.ToString());
            ShowInteractionHint();
        }

        public void ShowInteractionHint()
        {
            if(_isClosed)
            {
                Debug.Log("Open door");
            }
            else
            {
                Debug.Log("Close door");
            }
        }
        public void HideInteractionHint()
        {
            Debug.Log("Hide hint");
        }
        #endregion
    }
}
