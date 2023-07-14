using GameJam.Inputs;
using System;
using UnityEngine;

namespace GameJam.Player
{
    //Class to handle all movements
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class MovementController : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        [Header("Speed of player movement")]
        private float _horizontalSpeed;

        private float _xScale;
        private Rigidbody2D _playerRB;
        private PlayerInput _playerInput;
        #endregion

        #region Properties

        #endregion


        #region Built-in methods
        private void Start()
        {
            _playerRB = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();

            _xScale = transform.localScale.x;
        }

        void FixedUpdate()
        {
            HandleMovement();
        }
        #endregion


        #region Custom methods
        private void HandleMovement()
        {
            SetDirection(_playerInput.HorizontalInput);
            _playerRB.velocity = new Vector2(_horizontalSpeed * _playerInput.HorizontalInput, 0f);
        }

        private void SetDirection(float direction)
        {
            if(direction < 0)
                transform.localScale = new Vector3(_xScale, transform.localScale.y, transform.localScale.z);
            else if (direction > 0)
                transform.localScale = new Vector3(-_xScale, transform.localScale.y, transform.localScale.z);
        }
        #endregion
    }
}
