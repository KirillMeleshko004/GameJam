using GameJam.Core.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class BaseSceneController : MonoBehaviour
    {
        [SerializeField]
        protected List<GameObject> _scenesGameObjects = new();

        protected Queue<Action> _actionsToPerform = new Queue<Action>();

        public void NextAction()
        {
            _actionsToPerform.Dequeue().Invoke();
        }
    }
}
