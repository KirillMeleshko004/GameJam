using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class SwitchSofaSceneController : BaseSceneController
    {
        private void Start()
        {
            _actionsToPerform.Enqueue(ChangeSofa);
        }

        private void ChangeSofa()
        {
            _scenesGameObjects[0].SetActive(false);
            _scenesGameObjects[1].SetActive(true);
        }    
    }
}
