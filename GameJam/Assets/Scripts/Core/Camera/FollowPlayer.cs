using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Camera
{
    //Basic script for camera to follow player
    public class FollowPlayer : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float maxSpeed = 0.1f;

        [SerializeField]
        private float _maxLeft;
        [SerializeField]
        private float _maxRight;
        #endregion

        #region Built-in methods
        private void LateUpdate()
        {
            Vector3 target = new Vector3(_target.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, target, maxSpeed * Time.deltaTime);
            if (transform.position.x < _maxLeft)
                transform.position = new Vector3(_maxLeft, transform.position.y, transform.position.z);
            else if (transform.position.x > _maxRight)
                transform.position = new Vector3(_maxRight, transform.position.y, transform.position.z);
        }
        #endregion
    }
}