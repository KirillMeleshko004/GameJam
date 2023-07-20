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

        private void Start()
        {
            _actionsToPerform.Enqueue(SecondAction);
            _actionsToPerform.Enqueue(SecondAction);
        }


        public void NextAction()
        {
            _actionsToPerform.Dequeue().Invoke();
        }

        private void FirstAction()
        {
            FirstActionCouroutine();
        }
        private void FirstActionCouroutine()
        {

            //player movement with action example 
            Mover.AddMovement(_scenesGameObjects[0], _scenesGameObjects[2].transform.position);
        }
        private void SecondAction()
        {
            StartCoroutine(Second());
        }

        private IEnumerator Second()
        {
            GameObject derek = _scenesGameObjects[0];

            Mover.AddMovement(derek, _scenesGameObjects[3].transform.position);
            while(!Mover.IsAtTarget(derek))
                yield return new WaitForFixedUpdate();

            derek.GetComponent<NpcActionController>().PerfomAction("Sit Down");
        }
    }
}
