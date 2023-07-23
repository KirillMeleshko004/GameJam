using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.ExcelMinigame;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Items
{
    public class PC : BaseInteractable
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

        [SerializeField]
        private Vector3 _offset = Vector3.zero;

        [SerializeField]
        private UnityEvent OnWorkCompleted;


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
        private void ResetPc()
        {
            _objectsToInteract.Clear();
        }
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
            _objectsToInteract[_player].objectInfo.EnableInteractions();
            ShowInteractionHint();
        }
        private IEnumerator StartWorkForPlayer(InteractionObjectInfo interactionObject)
        {
            interactionObject.objectInfo.DisableMovements();
            interactionObject.objectInfo.DisableInteractions();


            Mover.AddMovement(interactionObject.objectInfo.gameObject,
                new Vector3(
                    transform.position.x,
                    interactionObject.objectInfo.PersonTransform.position.y,
                    interactionObject.objectInfo.PersonTransform.position.z
                    ) + _offset
                );
            while (!Mover.IsAtTarget(interactionObject.objectInfo.gameObject))
                yield return new WaitForFixedUpdate();

            if (interactionObject.objectInfo.gameObject.transform.localScale.x < 0f)
                interactionObject.objectInfo.gameObject.transform.localScale = new Vector3(
                    -interactionObject.objectInfo.gameObject.transform.localScale.x,
                    interactionObject.objectInfo.gameObject.transform.localScale.y,
                    interactionObject.objectInfo.gameObject.transform.localScale.z
                    );

            //Play sit down animation
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.SitDownToChair);

            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            //Play pc fade in animation (default animation on _pcDisplay active
            _pcDisplay.SetActive(true);
            yield return new WaitForSeconds(_startUpAnimTime / 2f);

            StartExcelGame();
        }
        private IEnumerator StartWorkForNpc(InteractionObjectInfo interactionObject)
        {
            Mover.AddMovement(interactionObject.objectInfo.gameObject,
                new Vector3(
                    transform.position.x,
                    interactionObject.objectInfo.PersonTransform.position.y,
                    interactionObject.objectInfo.PersonTransform.position.z
                    ) + _offset
                );
            while (!Mover.IsAtTarget(interactionObject.objectInfo.gameObject))
                yield return new WaitForFixedUpdate();

            if (interactionObject.objectInfo.gameObject.transform.localScale.x < 0f)
                interactionObject.objectInfo.gameObject.transform.localScale = new Vector3(
                    -interactionObject.objectInfo.gameObject.transform.localScale.x,
                    interactionObject.objectInfo.gameObject.transform.localScale.y,
                    interactionObject.objectInfo.gameObject.transform.localScale.z
                    );

            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.SitDownToChair);
            interactionObject.objectInfo.SetAnimTrigger(animInfo);
        }
        private void StartWork(InteractionObjectInfo interactionObject)
        {
            if (interactionObject.isPlayer) StartCoroutine(StartWorkForPlayer(interactionObject));
            else StartCoroutine(StartWorkForNpc(interactionObject));
        }
        private IEnumerator FinishWorkForPlayer(InteractionObjectInfo interactionObject)
        {
            OnWorkCompleted?.Invoke();


            interactionObject.objectInfo.DisableMovements();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            //Play pc fade out animation
            _pcDisplay.GetComponentInChildren<Animator>().SetTrigger(_animatorClosePcTriggerName);

            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            GameObject.Destroy(_excelGameInstance);
            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            _pcDisplay.SetActive(false);

            //Play stand up animation
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromChair);
            interactionObject.objectInfo.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            ResetPc();
            interactionObject.objectInfo.EnableMovements();

        }
        private void FinishWorkForNpc(InteractionObjectInfo interactionObject)
        {
            AnimInfo animInfo = interactionObject.objectInfo.AnimInfo.GetAnimationInfo(AnimationTypes.StandUpFromChair);
            interactionObject.objectInfo.SetAnimTrigger(animInfo);

            ResetPc();
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
            foreach (var kvp in _objectsToInteract)
                if (kvp.Value.isWorking && kvp.Key != sender) return;

            if (!_objectsToInteract.ContainsKey(sender))
            {
                _objectsToInteract.Add(sender, new InteractionObjectInfo(sender.GetComponent<PersonObject>(),
                    sender == _player));
            }

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

        public override void ShowInteractionHint()
        {
            foreach (var kvp in _objectsToInteract)
            {
                if (kvp.Value.isWorking && kvp.Key != _player) return;
            }

            if (_player == null) return;

            if (!_objectsToInteract.ContainsKey(_player))
                _objectsToInteract.Add(_player, new InteractionObjectInfo(_player.GetComponent<PersonObject>(), true));

            base._hintDisplay.SetActive(true);
            if (!_objectsToInteract[_player].workCompleted)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startWorkHint);
            else
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_finishWorkHint);
        }
        #endregion
    }
}
