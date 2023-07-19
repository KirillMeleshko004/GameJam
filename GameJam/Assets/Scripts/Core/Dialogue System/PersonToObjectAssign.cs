using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam.Core.Dialogue
{
    public class PersonToObjectAssign : MonoBehaviour
    {

        [SerializeField]
        private List<PersonToObject> _personToObjectList = new();

        public GameObject GetObjectToPerson(string person)
        {
            return (from pto in _personToObjectList where pto.Person == person select pto.Object).First<GameObject>();
        }
        public Vector3 GetOffsetForPerson(string person)
        {
            return (from pto in _personToObjectList where pto.Person == person select pto.DialogueOffset).First<Vector3>();
        }

        public Vector3 GetPositionWithOffset(string person)
        {
            return (from pto in _personToObjectList where pto.Person == person 
                    select pto.DialogueOffset + pto.Object.transform.position).First<Vector3>();
        }


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
            public Vector3 DialogueOffset { get { return _dialogueOffset; } }
        }
    }
}
