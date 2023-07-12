using GameJam.Inputs;
using UnityEngine;

namespace GameJam.Player
{
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
            Vector2 currentPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            Vector2 wantedPos = currentPos + (_horizontalSpeed * Time.deltaTime * _playerInput.HorizontalInput * Vector2.right);

            _playerRB.MovePosition(wantedPos);
        }
        #endregion
    }
}
