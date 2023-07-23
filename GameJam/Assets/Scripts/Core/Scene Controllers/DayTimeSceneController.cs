using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class DayTimeSceneController : BaseSceneController
    {
        [SerializeField]
        private GameObject _sky;

        [SerializeField]
        private Color _eveningSkyColor;

        public override void NextAction()
        {
            ChangeSkyToEvening();
        }

        private void ChangeSkyToEvening()
        {
            _sky.GetComponent<SpriteRenderer>().color = _eveningSkyColor;
        }
    }
}
