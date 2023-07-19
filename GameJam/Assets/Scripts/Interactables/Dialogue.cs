using GameJam.Core.Interactions;
using GameJam.Inputs;
using GameJam.ScriptableObjects.Dialogue;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameJam.Items
{

    public class Dialogue : MonoBehaviour, IInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [SerializeField]
        private TextAsset _dialogueText;

        [Header("Display, where interaction hint is showing")]
        [SerializeField]
        protected GameObject _hintDisplay;

        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;



        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _startDialogueHint = "Press E to start dialogue";
        [SerializeField]
        private string _continueDialogueHint = "Press E to continue";

        [SerializeField]
        private DialogueScript dialogue;

        #endregion


        #region Custom methods
       
        #endregion

        #region IInteractable realisation
        public GameObject InteractableObject { get { return this.gameObject; } }
        public void Interact()
        {
            if(dialogue != null)
            {
                if (!dialogue.IsDisplaying)
                {
                    DisablePlayerMovement();
                    dialogue.StartDialogue(_dialogueText);
                }
                else if (dialogue.IsTypingNow)
                    dialogue.SkipTextAnimation();
                else if (!dialogue.IsCompleted)
                    dialogue.ShowNextSentence();
                else
                {
                    dialogue.FinishDialogue();
                    EnablePlayerMovement();
                }
            }

            ShowInteractionHint();
        }

        public void ShowInteractionHint()
        {
            if (!_hintDisplay.activeSelf)
                _hintDisplay.SetActive(true);
            if(dialogue!= null)
            {
                if (!dialogue.IsDisplaying) _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startDialogueHint);
                else if (!dialogue.IsCompleted) _hintDisplay.GetComponent<HintDisplay>().DisplayHint(_continueDialogueHint);

            }

        }
        public void HideInteractionHint()
        {
            _hintDisplay.GetComponent<HintDisplay>().HideHint();
        }

        #endregion

        #region IPlayerMovementRestrictor implementation
        public void DisablePlayerMovement()
        {
            _playerInput.IsMovementEnabled = false;
        }

        public void EnablePlayerMovement()
        {
            _playerInput.IsMovementEnabled = true;
        }
        #endregion
    }
}
