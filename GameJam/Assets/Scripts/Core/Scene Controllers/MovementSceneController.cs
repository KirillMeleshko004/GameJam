using GameJam.Core.Movement;
using GameJam.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Core.SceneControllers
{
    public class MovementSceneController : BaseSceneController, ISerializationCallbackReceiver
    {
        [SerializeField]
        private GameObject _player;


        [Header("Action perfomed at the start of the scene")]
        [SerializeField]
        private List<MovementInfo> _movementActionsOnStart = new();

        [Header("Action perfomed during scene")]
        [SerializeField]
        private List<MovementInfo> _movementsToPerformHelper = new();

        private readonly Queue<MovementInfo> _movementsToPerform = new();



        private void Start()
        {
            foreach(var movement in _movementActionsOnStart)
                HandleMovement(movement);
        }


        public override void NextAction()
        {
            if(_movementsToPerform.Count != 0)
                HandleMovement(_movementsToPerform.Dequeue());

        }

        private void HandleMovement(MovementInfo actionInfo)
        {
            if(actionInfo.target == _player)
                StartCoroutine(HandleMovementForPlayer(actionInfo));
            else
                HandleMovementForNpc(actionInfo);
        }

        private IEnumerator HandleMovementForPlayer(MovementInfo actionInfo)
        {
            PersonObject player = _player.GetComponent<PersonObject>();
            player.DisableInteractions();
            player.DisableMovements();

            Mover.AddMovement(actionInfo.target, actionInfo.position);

            while(!Mover.IsAtTarget(actionInfo.target))
                yield return new WaitForFixedUpdate();

            player.EnableMovements();
            player.EnableInteractions();
        }
        private void HandleMovementForNpc(MovementInfo actionInfo)
        {
            Mover.AddMovement(actionInfo.target, actionInfo.position);
        }

        public void OnBeforeSerialize()
        {
            //No need
        }
        public void OnAfterDeserialize()
        {
            _movementsToPerform.Clear();
            foreach(var action in _movementsToPerformHelper)
                _movementsToPerform.Enqueue(action);
        }

        [Serializable]
        private sealed class MovementInfo
        {
            public GameObject target = null;
            public Vector3 position = Vector3.zero;
        }
    }
}
