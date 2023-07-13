using GameJam.Core.Interactions;
using ScriptableObjects.Readables;
using UnityEngine;

namespace GameJam.Items
{
    //class that represents interactable item
    public class Item : MonoBehaviour, IInteractable
    {
        #region Variables
        [SerializeField]
        private GameObject _readableDisplay;
        [SerializeField]
        private Readable _readable;

        private bool _isShowing = false;
        #endregion

        #region Properties
        #endregion

        #region Built-in methods
        private void Start()
        {
            InteractableObject = this.gameObject;
        }
        #endregion

        #region Custom methods
        private void ShowReadable()
        {
            _readableDisplay.GetComponent<ReadableDisplay>().DisplayReadable(_readable);
        }
        private void HideReadable()
        {
            _readableDisplay.GetComponent<ReadableDisplay>().HideReadable();
        }
        #endregion

        #region IInteractable realisation
        public GameObject InteractableObject { get; private set; }
        public void Interact()
        {
            Debug.Log("Interaction");
            if(!_isShowing)
                ShowReadable();
            else
                HideReadable();

            _isShowing = !_isShowing;
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
