using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.Dialogue
{
    public class PersonToObjectAssign : MonoBehaviour, ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<PersonToObject> _personToObjectListSerializationHelper = new List<PersonToObject>();

        private static readonly Dictionary<string, PersonToObject> _personToObjectDict = new();

        public static GameObject GetObjectToPerson(string person)
        {
            return _personToObjectDict[person].Object;
        }
        public static Vector3 GetOffsetForPerson(string person)
        {
            return _personToObjectDict[person].DialogueOffset;
        }

        public static void SetOffsetForName(string person, Vector3 newOffset)
        {
            _personToObjectDict[person].DialogueOffset = newOffset;
        }

        #region ISerializationCallbackReceiver implementation
        public void OnAfterDeserialize()
        {
            _personToObjectDict.Clear();
            foreach (var pto in _personToObjectListSerializationHelper)
                if(!_personToObjectDict.TryAdd(pto.Person, pto))
                    _personToObjectDict[pto.Person] = pto;
        }

        public void OnBeforeSerialize()
        {
            //No need
        }
        #endregion  

        [Serializable]
        private sealed class PersonToObject
        {
            [SerializeField]
            private string _person;
            [SerializeField]
            private GameObject _object;
            [SerializeField]
            private Vector3 _dialogueOffset;

            public string Person { get { return _person; } }
            public GameObject Object { get { return _object; } }
            public Vector3 DialogueOffset { get { return _dialogueOffset; } set { _dialogueOffset = value; } }
        }
    }
}
