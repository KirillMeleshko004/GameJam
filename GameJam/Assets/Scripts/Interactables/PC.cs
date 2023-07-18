using GameJam.Core.Interactions;
using GameJam.ExcelMinigame;
using GameJam.Inputs;
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
        protected GameObject _pcDisplay;
        [Header("PlayerInput component of current player")]
        [SerializeField]
        private PlayerInput _playerInput;

        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _startWorkHint = "Press E to start work";
        [SerializeField]
        private string _finishWorkHint = "Press E to finish work";

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
            _excelGameInstance.transform.localPosition = Vector3.zero;
            _excelGameInstance.GetComponentInChildren<ExcelTableGame>().GameCompleted += OnGameCompleted;
        }
        private void CloseExcelGame()
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            HideInteractionHint();
            GameObject.Destroy(_excelGameInstance);
        }

        private void OnGameCompleted()
        {
            Debug.Log("Completed");
            _workCompleted = true;
            ShowInteractionHint();
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
                DisablePlayerMovement();
                _isWorking = true;
                StartExcelGame();
            }
            else if(_isWorking && _workCompleted)
            {
                EnablePlayerMovement();
                CloseExcelGame();
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
