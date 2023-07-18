using GameJam.Core.Movement;
using GameJam.Inputs;
using UnityEngine;

namespace GameJam.Player
{
    //Class to handle all movements
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    public class MovementController : MonoBehaviour, IMovementController
    {
        #region Variables
        [SerializeField]
        private float _maxSpeed;
        [SerializeField]
        private Animator _playerAnim;
        [SerializeField]
        private string _animatorIsMovingBoolName = "isMoving";

        private float _xScale;
        private Rigidbody2D _playerRB;
        private PlayerInput _playerInput;
        #endregion

        #region Properties
        public float MaxSpeed { get { return _maxSpeed; } }

        public string AnimatorIsMovingBoolName { get { return _animatorIsMovingBoolName; } }
        #endregion


        #region Built-in methods
        private void Start()
        {
            _playerRB = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _playerAnim = GetComponent<Animator>();

            _xScale = transform.localScale.x;
        }

        void FixedUpdate()
        {
            HandleMovement();
            HandleAnimation();
            HandleDirection(_playerRB.velocity.x);
        }
        #endregion


        #region Custom methods
        private void HandleMovement()
        {
            if (!_playerInput.IsMovementEnabled) return;
            _playerRB.velocity = new Vector2(_maxSpeed * _playerInput.HorizontalInput, 0f);
        }
        private void HandleAnimation()
        {
            if (_playerRB.velocity != Vector2.zero)
                _playerAnim.SetBool(_animatorIsMovingBoolName, true);
            else
                _playerAnim.SetBool(_animatorIsMovingBoolName, false);
        }
        private void HandleDirection(float direction)
        {
            if (direction < 0)
                transform.localScale = new Vector3(_xScale, transform.localScale.y, transform.localScale.z);
            else if (direction > 0)
                transform.localScale = new Vector3(-_xScale, transform.localScale.y, transform.localScale.z);
        }
        #endregion
    }
}
