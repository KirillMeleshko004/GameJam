using UnityEngine;

namespace GameJam.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        #region Variables
        private float _horizontalInput;

        #endregion

        #region Properties
        public float HorizontalInput { get { return _horizontalInput; } }
        #endregion


        #region Built-in methods
        void Update()
        {
            HandleHorizontalInput();
        }
        #endregion


        #region Custom methods
        private void HandleHorizontalInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
        }
        #endregion
    }
}

