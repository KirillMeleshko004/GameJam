using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{
    public class DisablingSceneController : BaseSceneController, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<GameObject> _objectsToDisableHelperList = new();

        private readonly Queue<GameObject> _objectsToDisable = new();

        public override void NextAction()
        {   
            if (_objectsToDisable.Count > 0)
                _objectsToDisable.Dequeue().SetActive(false);
        }
        public void OnBeforeSerialize()
        {
            //No need
        }

        public void OnAfterDeserialize()
        {
            _objectsToDisable.Clear();
            foreach (var obj in _objectsToDisableHelperList)
                _objectsToDisable.Enqueue(obj);
        }
    }
}
