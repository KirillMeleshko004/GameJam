using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Core.Controllers
{
    public class NpcActionController : MonoBehaviour, ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<ActionInfo> _actionsList = new();

        private readonly Dictionary<string, UnityEvent<GameObject>> _actions = new();

        #region ISerializationCallbackReceiver implementation
        public void OnAfterDeserialize()
        {
            _actions.Clear();
            foreach (var actInf in _actionsList)
                _actions.Add(actInf.ActionName, actInf.Action);
        }

        public void OnBeforeSerialize()
        {
            //No need
        }
        #endregion

        public void PerfomAction(string actionName)
        {
            _actions[actionName]?.Invoke(gameObject);
        }


        [Serializable]
        private sealed class ActionInfo
        {
            [SerializeField]
            private string _actionName;
            [SerializeField]
            private UnityEvent<GameObject> _action;

            public string ActionName { get { return _actionName; } }
            public UnityEvent<GameObject> Action { get { return _action; } }
        }
    }
}
