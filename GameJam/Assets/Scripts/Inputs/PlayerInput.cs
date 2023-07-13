using UnityEngine;

namespace GameJam.Inputs
{
    //Wrapper-class to get input
    public class PlayerInput : MonoBehaviour
    {
        #region Variables
        private float _horizontalInput;
        private bool _basicInteractionInput;

        #endregion

        #region Properties
        public float HorizontalInput { get { return _horizontalInput; } }
        public bool BasicInteractionInput { get { return _basicInteractionInput; } }
        #endregion


        #region Built-in methods
        void Update()
        {
            HandleHorizontalInput();
            HandleInteractionInput();
        }
        #endregion


        #region Custom methods
        private void HandleHorizontalInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
        }

        private void HandleInteractionInput()
        {
            _basicInteractionInput = Input.GetKeyDown(KeyCode.E);
        }
        #endregion
    }
}

