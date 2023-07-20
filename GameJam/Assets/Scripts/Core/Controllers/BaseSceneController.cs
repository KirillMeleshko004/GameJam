using GameJam.Core.Movement;
using GameJam.Inputs;
using GameJam.Items;
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
            StartCoroutine(FirstActionCouroutine());
        }
        private IEnumerator FirstActionCouroutine()
        {
            //player movement example

            GameObject player = _scenesGameObjects[1];
            GameObject sofa = _scenesGameObjects[2];

            player.GetComponent<PlayerInput>().IsMovementEnabled = false;

            Mover.AddMovement(player, new Vector3(sofa.transform.position.x, player.transform.position.y, player.transform.position.z));

            while(Mover.IsAtTarget(player))
                yield return new WaitForFixedUpdate();

            player.GetComponent<PlayerInput>().IsMovementEnabled = true;

            sofa.GetComponent<Sofa>().Interact(player);
        }
        private void SecondAction()
        {
            Debug.Log("Second action");
        }
    }
}
