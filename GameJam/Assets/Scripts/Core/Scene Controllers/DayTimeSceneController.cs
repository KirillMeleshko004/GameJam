using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class DayTimeSceneController : BaseSceneController
    {
        [SerializeField]
        private GameObject _sky;
        [SerializeField]
        private GameObject _city;

        [SerializeField]
        private Color _eveningSkyColor;
        [SerializeField]
        private Color _cityEveningColor;

        public override void NextAction()
        {
            ChangeSkyToEvening();
        }

        private void ChangeSkyToEvening()
        {
            _sky.GetComponent<SpriteRenderer>().color = _eveningSkyColor;
            _city.GetComponent<SpriteRenderer>().color = _cityEveningColor;
        }
    }
}
