using GameJam.Inputs;
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
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        #endregion


        #region Custom methods
        private void HandleMovement()
        {
            _playerRB.velocity = new Vector2(_horizontalSpeed * _playerInput.HorizontalInput, 0f);
        }
        #endregion
    }
}
