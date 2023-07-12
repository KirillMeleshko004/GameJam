using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Camera
{
    public class FollowPlayer : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float maxSpeed = 0.1f;
        #endregion

        #region Built-in methods
        private void FixedUpdate()
        {
            Vector3 target = new Vector3(_target.position.x, _target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, target, maxSpeed);
        }
        #endregion
    }
}