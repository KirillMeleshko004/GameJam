using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class DoorActivationSceneController : BaseSceneController
    {
        [SerializeField]
        private GameObject _door;

        protected Queue<Action> _actionsToPerform = new();
        private void Start()
        {
            _actionsToPerform.Enqueue(ActivateDoor);
        }

        private void ActivateDoor()
        {
            _door.SetActive(true);
        }

        public override void NextAction()
        {
            if(_actionsToPerform.Count > 0)
                _actionsToPerform.Dequeue()?.Invoke();
        }
    }
}
