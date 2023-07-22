using GameJam.Items;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static GameJam.Items.DialogueScript;

namespace GameJam.Core.SceneControllers
{

    public class DialogueActionSceneController : MonoBehaviour
    {

        [SerializeField]
        private DialogueScript _dialogue;

        [SerializeField]
        private List<ActionToDialgueLine> _actionToLine = new();

        private void LateUpdate()
        {
            if (_actionToLine.Count != 0)
                HandleActions();
        }


        private void HandleActions()
        {
            for(int i = 0; i < _actionToLine.Count; i++)
            {
                if(AreLinesSame(_dialogue.CurrentLine, _actionToLine[i].dialogueLine))
                {
                    StartCoroutine(PerformAction(_actionToLine[i]));
                    _actionToLine.Remove(_actionToLine[i]);
                }
            }
        }

        private bool AreLinesSame(DialogueLine firstLine, DialogueLine secondLine)
        {
            return firstLine?.Line.ToLower().Trim() == secondLine?.Line.ToLower().Trim() && 
                firstLine?.Person.ToLower().Trim() == secondLine?.Person.ToLower().Trim();
        }
        
        private IEnumerator PerformAction(ActionToDialgueLine atd)
        {

            if (atd._suspendTime > 0)
            {
                _dialogue.RequestDialogueSuspension(atd._suspendTime);
                while (!_dialogue.IsSuspended)
                    yield return new WaitForFixedUpdate();
            }

            atd.actionWithoutArguments?.Invoke();

            while (atd.arguments.Count != 0)
            {
                atd.action?.Invoke(atd.arguments[0]);
                atd.arguments.RemoveAt(0);
            }
        }

        [Serializable]
        private sealed class ActionToDialgueLine
        {
            public DialogueLine dialogueLine = null;
            public UnityEvent<GameObject> action = null;
            public List<GameObject> arguments = new ();


            public UnityEvent actionWithoutArguments = null;

            [Header("Negative is no suspend")]
            public float _suspendTime = -1;
        }
    }
}
