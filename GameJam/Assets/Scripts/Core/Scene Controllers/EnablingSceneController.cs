using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class EnablingSceneController : BaseSceneController, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<GameObject> _objectsToEnableHelperList = new();

        private readonly Queue<GameObject> _objectsToEnable = new();

        public override void NextAction()
        {
            if (_objectsToEnable.Count > 0)
                _objectsToEnable.Dequeue()?.SetActive(true);
        }
        public void OnBeforeSerialize()
        {
            //No need
        }

        public void OnAfterDeserialize()
        {
            _objectsToEnable.Clear();
            foreach(var  obj in _objectsToEnableHelperList)
                _objectsToEnable.Enqueue(obj);
        }
    }
}
