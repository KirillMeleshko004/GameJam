using GameJam.Core.Movement;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam.Core.Controllers
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class NpcMovementController : MonoBehaviour, IMovementController
    {
        [SerializeField]
        private float _maxSpeed;
        [SerializeField]
        private string _animatorIsMovingBoolName;

        private Animator _animator;
        private Rigidbody2D _rigidbody;

        private float _xScale;
        #region Properties
        public float MaxSpeed {get { return _maxSpeed; }}
        public string AnimatorIsMovingBoolName { get { return _animatorIsMovingBoolName; } }
        #endregion

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();

            _xScale = transform.localScale.x;
        }

        private void Update()
        {
            HandleDirection(_rigidbody.velocity.x);
        }

        public void MoveToPosition(Vector3 target)
        {
            Mover.AddMovement(gameObject, target);
        }

        private void HandleDirection(float direction)
        {
            if (direction < 0)
                transform.localScale = new Vector3(Mathf.Abs(_xScale), transform.localScale.y, transform.localScale.z);
            else if (direction > 0)
                transform.localScale = new Vector3(-Mathf.Abs(_xScale), transform.localScale.y, transform.localScale.z);
        }
    }
}
