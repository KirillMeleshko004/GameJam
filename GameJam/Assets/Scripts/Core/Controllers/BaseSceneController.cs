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

        private void Start()
        {
            _actionsToPerform.Enqueue(FirstAction);
            _actionsToPerform.Enqueue(SecondAction);
        }


        public void NextAction()
        {
            _actionsToPerform.Dequeue().Invoke();
        }

        private void FirstAction()
        {
            Debug.Log("First action");
        }
        private void SecondAction()
        {
            Debug.Log("Second action");
        }
    }
}
