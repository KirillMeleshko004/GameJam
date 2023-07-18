using GameJam.Core.Interactions;
using GameJam.ExcelMinigame;
using GameJam.Inputs;
using System.Collections;
using UnityEngine;

namespace GameJam.Items
{
    public class PC : BaseInteractable, IPlayerMovementRestrictor
    {
        #region Variables
        [Header("Prefab of excel game")]
        [SerializeField]
        private GameObject _excelGamePrefab;
        [Header("Display, where pc interface is running")]
        [SerializeField]
        private GameObject _pcDisplay;
        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _startWorkHint = "Press E to start work";
        [SerializeField]
        private string _finishWorkHint = "Press E to finish work";

        [Header("Animator of the player")]
        [SerializeField]
        private Animator _playerAnim;
        [Header("Bool trigger name to play different sittin animation")]
        [SerializeField]
        private string _animatorIsSittingBoolName = "isSittingOnChair";
        [Header("Length of animation clips")]
        [SerializeField]
        private float _sitDownAnimTime = 1.2f;
        [SerializeField]
        private float _startUpAnimTime = 3f;

        [SerializeField]
        private string _animatorClosePcTriggerName = "closePc";

        private bool _isWorking = false;
        private bool _workCompleted = false;
        private GameObject _excelGameInstance;

        #endregion

        #region Properties
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
            _workCompleted = true;
            ShowInteractionHint();
        }

        private IEnumerator StartWork()
        {
            DisablePlayerMovement();
            HideInteractionHint();
            
            //Play sit down animation
            _playerAnim.SetBool(_animatorIsSittingBoolName, true);
            yield return new WaitForSeconds(_sitDownAnimTime);

            //Play pc fade in animation (default animation on _pcDisplay active
            _pcDisplay.SetActive(true);
            yield return new WaitForSeconds(_startUpAnimTime / 2f);
            StartExcelGame();
        }
        private IEnumerator FinishWork()
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
            _playerAnim.SetBool(_animatorIsSittingBoolName, false);
            yield return new WaitForSeconds(_sitDownAnimTime);

            EnablePlayerMovement();
        }

        #endregion

        #region BaseInteractable implementation
        public override GameObject InteractableObject {  get { return this.gameObject; } }

        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        public override void Interact()
        {
            if(!_isWorking && !_workCompleted)
            {
                _isWorking = true;
                StartCoroutine(StartWork());
            }
            else if(_isWorking && _workCompleted)
            {
                StartCoroutine(FinishWork());
            }
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            if (!_workCompleted)
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_startWorkHint);
            else
            {
                base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_finishWorkHint);
            }
        }

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
        #endregion
    }
}
