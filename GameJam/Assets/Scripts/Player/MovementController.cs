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
        private float _horizontalSpeed;

        private Rigidbody2D _playerRB;
        private PlayerInput _playerRInput;
        #endregion

        #region Properties

        #endregion


        #region Built-in methods
        private void Start()
        {
            _playerRB = GetComponent<Rigidbody2D>();
            _playerRInput = GetComponent<PlayerInput>();
        }

        void Update()
        {
            HandleMovement();
        }
        #endregion


        #region Custom methods
        private void HandleMovement()
        {
            Vector2 wantedPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) +
                _horizontalSpeed * _playerRInput.HorizontalInput * Vector2.right;
            _playerRB.MovePosition(wantedPos);
        }
        #endregion
    }
}
