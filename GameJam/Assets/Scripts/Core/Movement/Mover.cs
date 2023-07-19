using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Movement
{
    public class Mover : MonoBehaviour
    {
        #region Variables
        private static readonly List<MovableInfo> _objectsToMove = new();

        private static readonly Dictionary<GameObject, MovableInfo> _history = new();

        [Header("Accuracy of movement")]
        [SerializeField]
        private float _epsilon;
        #endregion

        #region Properties
        #endregion

        #region Built-in methods

        void FixedUpdate()
        {
            if (_objectsToMove.Count != 0)
            {
                for (int i = 0; i < _objectsToMove.Count; i++)
                    HandleMovement(_objectsToMove[i]);
            }
        }

        private void Update()
        {
            if (_objectsToMove.Count != 0)
            {
                for (int i = 0; i < _objectsToMove.Count; i++)
                    CheckTargetReach(_objectsToMove[i]);
            }
        }
        #endregion

        #region Custom methods
        private void Move(Rigidbody2D rigidbody, Vector3 target, float maxSpeed)
        {
            Vector2 directionSpeed = target - rigidbody.transform.position;

            float speedMultiplier = maxSpeed / directionSpeed.magnitude;
            directionSpeed = new Vector2(speedMultiplier * directionSpeed.x, speedMultiplier * directionSpeed.y);

            rigidbody.velocity = directionSpeed;
        }
        private void CheckTargetReach(MovableInfo movable)
        {
            if (Mathf.Abs(movable.MovableTransform.position.x - movable.Target.x) < _epsilon &&
                Mathf.Abs(movable.MovableTransform.position.y - movable.Target.y) < _epsilon)
                movable.TargetReached = true;
            movable.MovableRb.velocity = Vector3.zero;
        }
        private void HandleMovement(MovableInfo movableInfo)
        {
            if (movableInfo.TargetReached)
            {
                movableInfo.MovableRb.velocity = Vector3.zero;
                movableInfo.MovableAnimator.SetBool(movableInfo.AnimatorIsMovingBoolName, false);
                _objectsToMove.Remove(movableInfo);
                return;
            }

            movableInfo.MovableAnimator.SetBool(movableInfo.AnimatorIsMovingBoolName, true);
            Move(movableInfo.MovableRb, movableInfo.Target, movableInfo.MaxSpeed);
        }
        public static void AddMovement(GameObject objectToMove, Vector3 target)
        {
            MovableInfo movableInfo;
            if (_history.ContainsKey(objectToMove))
                movableInfo = _history[objectToMove];
            else
            {
                movableInfo = new MovableInfo(objectToMove);
                _history.Add(objectToMove, movableInfo);
            }

            movableInfo.Target = target;
            movableInfo.PrevPosition = movableInfo.MovableTransform.position;
            movableInfo.TargetReached = false;

            _objectsToMove.Add(movableInfo);
        }

        public static bool IsAtTarget(GameObject obj)
        {
            bool isAtTarget = _history[obj].TargetReached;
            if (!isAtTarget) return false;
            else
            {
                _history[obj].TargetReached = false;
                return true;
            }
        }
         
        #endregion

        private sealed class MovableInfo
        {
            #region Movable object info
            public Rigidbody2D MovableRb { get; private set; }
            public Transform MovableTransform { get; private set; }
            public float MaxSpeed { get; private set; }

            //Animation info
            public Animator MovableAnimator { get; private set; }
            public string AnimatorIsMovingBoolName { get; private set; }
            #endregion

            public Vector3 Target { get; set; }
            public bool TargetReached { get; set; } = false;
            //Info for return
            public Vector3 PrevPosition { get; set; }

            public MovableInfo(GameObject movable)
            {
                MovableRb = movable.GetComponent<Rigidbody2D>();
                MovableTransform = movable.transform;
                MaxSpeed = movable.GetComponent<IMovementController>()?.MaxSpeed ?? 2.3f;
                MovableAnimator = movable.GetComponent<Animator>();
                AnimatorIsMovingBoolName = movable.GetComponent<IMovementController>()?.AnimatorIsMovingBoolName ?? "isMoving";
            }
        }
    }
}
