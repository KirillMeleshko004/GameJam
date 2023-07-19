using GameJam.Core.Interactions;
using GameJam.ExcelMinigame;
using GameJam.Inputs;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Items
{
    public class PC : BaseInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [SerializeField]
        private GameObject _player;

        [Header("Prefab of excel game")]
        [SerializeField]
        private GameObject _excelGamePrefab;
        [Header("Display, where pc interface is running")]
        [SerializeField]
        private GameObject _pcDisplay;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _startWorkHint = "Press E to start work";
        [SerializeField]
        private string _finishWorkHint = "Press E to finish work";

        [Header("Length of animation clips")]
        [SerializeField]
        private float _startUpAnimTime = 3f;

        [SerializeField]
        private string _animatorClosePcTriggerName = "closePc";

        private GameObject _excelGameInstance;

        #endregion

        #region Properties
        #endregion

        #region Interactions info
        private readonly Dictionary<GameObject, InteractionObjectInfo> _objectsToInteract = new();
        private sealed class InteractionObjectInfo
        {
            public PersonObject objectInfo;
            public bool isWorking = false;
            public bool workCompleted = false;
            public bool isPlayer;

            public InteractionObjectInfo(PersonObject objectInfo, bool isPlayer,
                bool isWorking = false, bool workCompleted = false)
            {
                this.objectInfo = objectInfo;
                this.isWorking = isWorking;
                this.workCompleted = workCompleted;
                this.isPlayer = isPlayer;
            }
        }
        #endregion

        #region Custom methods
        private void StartExcelGame()
        {
            HideInteractionHint();
            _excelGameInstance = Instantiate(_excelGamePrefab, Vector3.zero, Quaternion.identity, _pcDisplay.transform);
            _excelGameInstance.transform.SetAsFirstSibling();
            _excelGameInstance.transform.localPosition = Vector3.zero;
            _excelGameInstance.GetComponentInChildren<ExcelTableGame>().GameCompleted += OnGameCompleted;
        }

        private void OnGameCompleted()
        {
            _objectsToInteract[_player].workCompleted = true;
            ShowInteractionHint(_player);
        }

        private void SetAnimTrigger(Animator animator, AnimInfo animInfo)
        {
            if (animInfo.TriggerType == TriggerTypes.Bool)
                animator.SetBool(animInfo.TriggerValueName, animInfo.TriggerValue);
            else
                animator.SetTrigger(animInfo.TriggerValueName);
        }
        private IEnumerator StartWorkForPlayer(InteractionObjectInfo interactionObject)
        {
            DisablePlayerMovement(interactionObject.objectInfo.PlayerInput);
            HideInteractionHint();

            //Play sit down animation
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.SitDownToChair);

            SetAnimTrigger(interactionObject.objectInfo.PersonAnim, animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            //Play pc fade in animation (default animation on _pcDisplay active
            _pcDisplay.SetActive(true);
            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            StartExcelGame();
        }
        private void StartWorkForNpc(InteractionObjectInfo interactionObject)
        {
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.SitDownToChair);
            SetAnimTrigger(interactionObject.objectInfo.PersonAnim, animInfo);
        }
        private void StartWork(InteractionObjectInfo interactionObject)
        {
            if (interactionObject.isPlayer) StartCoroutine(StartWorkForPlayer(interactionObject));
            else StartWorkForNpc(interactionObject);
        }
        private IEnumerator FinishWorkForPlayer(InteractionObjectInfo interactionObject)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            HideInteractionHint();

            //Play pc fade out animation
            _pcDisplay.GetComponentInChildren<Animator>().SetTrigger(_animatorClosePcTriggerName);

            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            GameObject.Destroy(_excelGameInstance);
            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            _pcDisplay.SetActive(false);

            //Play stand up animation
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromChair);
            SetAnimTrigger(interactionObject.objectInfo.PersonAnim, animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            EnablePlayerMovement(interactionObject.objectInfo.PlayerInput);
        }
        private void FinishWorkForNpc(InteractionObjectInfo interactionObject)
        {
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromChair);
            SetAnimTrigger(interactionObject.objectInfo.PersonAnim, animInfo);
        }

        private void FinishWork(InteractionObjectInfo interactionObject)
        {
            if (interactionObject.isPlayer) StartCoroutine(FinishWorkForPlayer(interactionObject));
            else FinishWorkForNpc(interactionObject);
        }
        #endregion

        #region BaseInteractable implementation
        public override GameObject InteractableObject {  get { return this.gameObject; } }

        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        public override void Interact(GameObject sender)
        {
            if (!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(),
                    sender == _player));

            if (!_objectsToInteract[sender].isWorking && !_objectsToInteract[sender].workCompleted)
            {
                _objectsToInteract[sender].isWorking = true;
                StartWork(_objectsToInteract[sender]);
            }
            else if(_objectsToInteract[sender].isWorking && _objectsToInteract[sender].workCompleted)
            {
                FinishWork(_objectsToInteract[sender]);
            }
        }

        public override void ShowInteractionHint(GameObject sender)
        {
            if (!_objectsToInteract.ContainsKey(sender))
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(),
                    sender == _player));
            base._hintDisplay.SetActive(true);
            if (!_objectsToInteract[sender].workCompleted)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startWorkHint);
            else
            {
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_finishWorkHint);
            }
        }

        #region IPlayerMovementRestrictor implementation
        public void DisablePlayerMovement(PlayerInput input)
        {
            input.IsMovementEnabled = false;
            input.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        public void EnablePlayerMovement(PlayerInput input)
        {
            input.IsMovementEnabled = true;
        }
        #endregion
        #endregion
    }
}
