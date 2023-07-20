using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Inputs;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace GameJam.Items
{
    public class Sofa : BaseInteractable
    {
        #region Variables
        [SerializeField]
        private GameObject _player;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _sitDownHint = "Prees E to sit down";
        [SerializeField]
        private string _standUpHint = "Prees E to stand up";
        [SerializeField]
        private string _shootHint = "Prees E to shoot";

        [Header("Is last scene")]
        [SerializeField]
        private bool _isDayX = false;
        #endregion

        #region Properties
        #endregion

        #region Interactions info
        private readonly Dictionary<GameObject, InteractionObjectInfo> _objectsToInteract = new();
        private sealed class InteractionObjectInfo
        {
            public PersonObject objectInfo;
            public bool isSitting = false;

            public InteractionObjectInfo(PersonObject objectInfo, bool isSitting = false)
            {
                this.objectInfo = objectInfo;
                this.isSitting = isSitting;
            }
        }
        #endregion

        #region Custom methods
        private IEnumerator StandUp(InteractionObjectInfo interactionObject)
        {
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromSofa);
            interactionObject.objectInfo.DisableInteractions();
            //StartCoroutine(DisableColliderForTime(animInfo.AnimationLength));

            interactionObject.objectInfo.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);

            interactionObject.objectInfo.EnableInteractions();
            interactionObject.objectInfo.EnableMovements();
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
                    )
                );

            while(!Mover.IsAtTarget(interactionObject.objectInfo.gameObject))
            {
                yield return new WaitForFixedUpdate();
            }

            //StartCoroutine(DisableColliderForTime(animInfo.AnimationLength));

            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            interactionObject.objectInfo.EnableInteractions();
        }

        private void Shoot(InteractionObjectInfo interactionObject)
        {
            transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(false);

            Suicide player = interactionObject.objectInfo.gameObject.GetComponent<Suicide>();
            player?.ShootDown();

        }

        //private IEnumerator DisableColliderForTime(float time)
        //{
        //    transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(false);
        //    yield return new WaitForSeconds(time);
        //    transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject.SetActive(true);
        //}
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact(GameObject sender)
        {
            if(!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>()));


            if (!_objectsToInteract[sender].isSitting)
                StartCoroutine(SitDown(_objectsToInteract[sender]));
            else if (!_isDayX)
                StartCoroutine(StandUp(_objectsToInteract[sender]));
            else
                Shoot(_objectsToInteract[sender]);

            _objectsToInteract[sender].isSitting = !_objectsToInteract[sender].isSitting;
        }

        public override void ShowInteractionHint(GameObject sender)
        {
            if (sender != _player) return;

            if (!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>()));

            base._hintDisplay.SetActive(true);
            if (!_objectsToInteract[sender].isSitting)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sitDownHint);
            else if (!_isDayX)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_standUpHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_shootHint);
        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        #endregion
    }
}
