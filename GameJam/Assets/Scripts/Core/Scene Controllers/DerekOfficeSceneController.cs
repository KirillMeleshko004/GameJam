using GameJam.Core.Controllers;
using GameJam.Core.Movement;
using GameJam.Items;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class DerekOfficeSceneController : BaseSceneController
    {
        [SerializeField]
        private PersonObject _derek;
        [SerializeField]
        private GameObject _derekCup;

        [SerializeField]
        private GameObject _derekPc;

        [SerializeField]
        private Vector3 _targetPos;

        [SerializeField]
        private GameObject _player;
        [SerializeField]
        private float _drinkDelay;
        [SerializeField]
        private Table _table;

        private readonly Queue<Action> _actionsToPerform = new();

        private void Start()
        {
            _actionsToPerform.Enqueue(DerekCheckWifeAction);
        }

        public override void NextAction()
        {
            _actionsToPerform.TryDequeue(out Action action);
            action?.Invoke();
        }


        private void DerekCheckWifeAction()
        {
            StartCoroutine(DerekActionsChain());
        }

        private IEnumerator DerekActionsChain()
        {
            AnimInfo animInfo = _derek.AnimInfo.GetAnimationInfo(AnimationTypes.LeaveTable);
            _derek.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);
            _derekCup.SetActive(true);

            Mover.AddMovement(_derek.gameObject, _targetPos);

            StartCoroutine(PlayerDrink());

            while (!Mover.IsAtTarget(_derek.gameObject))
                yield return new WaitForFixedUpdate();

            NpcActionController actionController = GetComponent<NpcActionController>();
            actionController.PerfomAction("sit down");
        }
        private IEnumerator PlayerDrink()
        {
            yield return new WaitForSeconds(_drinkDelay);
            _table.Drink(_player);
            yield return new WaitForSeconds(_drinkDelay);
            _table.Drink(_player);
        }
    }

}