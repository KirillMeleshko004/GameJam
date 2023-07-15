using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace GameJam.ExcelMinigame
{
    public class Calculator : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        TMP_InputField _inputField;

        private float _firstValue;
        private float _secondValue;
        private float _result;
        private string _operation;
        #endregion

        #region Properties
        //Binded to display
        public string ResultString { get; set; } = "0";
        #endregion


        #region Custom Methods
        public void OnNumberClick(int number)
        {
            if(_inputField.text == "0")
            {
                _inputField.text = string.Empty;
            }
            _inputField.text += number;
            Debug.Log(ResultString);
        }
        public void OnOperationClick(string operation)
        {
            _firstValue = float.Parse(ResultString);
            _operation = operation;
            _inputField.text = "0";
        }
        public void OnEqualsClick()
        {
            if (_operation == string.Empty) return;

            _secondValue = float.Parse(ResultString);
            
            switch(_operation)
            {
                case "+": _result = _firstValue + _secondValue; break;
                case "-": _result = _firstValue - _secondValue; break;
                case "/": _result = _firstValue / _secondValue; break;
                case "*": _result = _firstValue * _secondValue; break;
            }

            _operation = "";

            _inputField.text = _result.ToString();

            _firstValue = _result;
        }

        public void OnClearClick()
        {
            _result = 0;
            _firstValue = 0;
            _secondValue = 0;
            _operation = "";
            _inputField.text = "0";
        }
        public void OnClearEntryClick()
        {
            _inputField.text = "0";
        }
        #endregion
    }
}
