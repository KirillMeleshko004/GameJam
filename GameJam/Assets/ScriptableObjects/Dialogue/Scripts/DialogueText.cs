using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam.ScriptableObjects.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class DialogueText : ScriptableObject
    {
    }
    public class DialogueLine
    {
        [SerializeField]
        private string _person;
        [SerializeField]
        private string _line;

        public string Person { get { return _person; } private set { _person = value; } }
        public string Line { get { return _line; } private set { _line = value; } }

        public DialogueLine (string person, string line)
        {
            Person = person;
            Line = line;
        }
    }
}
