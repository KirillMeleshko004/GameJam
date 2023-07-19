using GameJam.Core.Movement;
using UnityEngine;

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

        #region Properties
        public float MaxSpeed {get { return _maxSpeed; }}
        public string AnimatorIsMovingBoolName { get { return _animatorIsMovingBoolName; } }
        #endregion

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        public void MoveToPosition(Vector3 target)
        {
            Mover.AddMovement(gameObject, target);
        }
    }
}
