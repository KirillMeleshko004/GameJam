using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class SwitchSofaSceneController : BaseSceneController
    {
        [SerializeField]
        private GameObject _sofa;
        [SerializeField]
        private GameObject _sleepingSofa;


        protected Queue<Action> _actionsToPerform = new();

        private void Start()
        {
            _actionsToPerform.Enqueue(ChangeSofaToSleeping);
        }

        private void ChangeSofaToSleeping()
        {
            _sofa.SetActive(false);
            _sleepingSofa.SetActive(true);
        }

        public override void NextAction()
        {
            if(_actionsToPerform.Count != 0 )
                _actionsToPerform.Dequeue()?.Invoke();
        }
    }
}
