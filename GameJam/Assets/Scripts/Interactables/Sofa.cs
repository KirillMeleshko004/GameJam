using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Items
{
    public class Sofa : BaseInteractable
    {
        #region Variables

        [SerializeField]
        private DialogueScript _dialogue;
        [SerializeField]
        private TextAsset _dialogueText = null;

        [SerializeField]
        private GameObject _player;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _sitDownHint = "Prees E to sit down";
        [SerializeField]
        private string _standUpHint = "Prees E to stand up";
        [SerializeField]
        private string _shootHint = "Prees E to shoot";
        [SerializeField]
        private string _nextLineHint = "Prees E to continue";

        [Header("Is last scene")]
        [SerializeField]
        private bool _isDayX = false;


        [SerializeField]
        private Vector3 _offset = Vector3.zero;
        #endregion

        #region Properties
        #endregion

        #region Interactions info
        private readonly Dictionary<GameObject, InteractionObjectInfo> _objectsToInteract = new();
        private sealed class InteractionObjectInfo
        {
            public PersonObject objectInfo;
            public bool isSitting = false;
            public bool canLeave = false;

            public InteractionObjectInfo(PersonObject objectInfo, bool canLeave)
            {
                this.objectInfo = objectInfo;
                this.canLeave = canLeave;
            }
        }
        #endregion

        #region Custom methods
        private void ResetSofa()
        {
            _objectsToInteract.Clear();
        }
        private IEnumerator StandUp(InteractionObjectInfo interactionObject)
        {
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromSofa);
            interactionObject.objectInfo.DisableInteractions();

            interactionObject.objectInfo.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);

            interactionObject.objectInfo.EnableInteractions();
            interactionObject.objectInfo.EnableMovements();
            ResetSofa();
        }
        private IEnumerator SitDown(InteractionObjectInfo interactionObject)
        {   
            interactionObject.objectInfo.DisableMovements();
            interactionObject.objectInfo.DisableInteractions();

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.SitDownToSofa);

            Mover.AddMovement(interactionObject.objectInfo.gameObject, 
                new Vector3(
                    transform.position.x,
                    interactionObject.objectInfo.PersonTransform.position.y,
                    interactionObject.objectInfo.PersonTransform.position.z
                    ) + _offset
                );

            while(!Mover.IsAtTarget(interactionObject.objectInfo.gameObject))
            {
                yield return new WaitForFixedUpdate();
            }


            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            if(_dialogueText!= null)
                _dialogue.StartDialogue(_dialogueText);

            interactionObject.objectInfo.EnableInteractions();
        }

        private void Shoot(InteractionObjectInfo interactionObject)
        {
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(false);

            Suicide player = interactionObject.objectInfo.gameObject.GetComponent<Suicide>();
            player?.ShootDown();

        }


        private void HandleDialogue()
        {
            if (_dialogue.IsCompleted)
            {
                _dialogue.FinishDialogue();
                _objectsToInteract[_player].canLeave = true;
            }
            else
            {
                _dialogue.ShowNextSentence();
            }
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact(GameObject sender)
        {
            if(!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(), 
                    _dialogueText == null || _dialogue.IsCompleted));


            if (!_objectsToInteract[sender].isSitting)
            {
                StartCoroutine(SitDown(_objectsToInteract[sender]));
                _objectsToInteract[sender].isSitting = !_objectsToInteract[sender].isSitting;
            }

            else if (_isDayX)
                Shoot(_objectsToInteract[sender]);

            else if (!_objectsToInteract[sender].canLeave)
                HandleDialogue();
            else
                StartCoroutine(StandUp(_objectsToInteract[sender]));


            ShowInteractionHint();
        }

        public override void ShowInteractionHint()
        {
            if (_player == null) return;

            if (!_objectsToInteract.ContainsKey(_player))
                _objectsToInteract.Add(_player, new InteractionObjectInfo(_player.GetComponent<PersonObject>(), 
                    _dialogueText == null || _dialogue.IsCompleted));

            base._hintDisplay.SetActive(true);
            if (!_objectsToInteract[_player].isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else if (_objectsToInteract[_player].canLeave)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
            else if(_isDayX)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_shootHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_nextLineHint);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        #endregion
    }
}
