using TMPro;
using UnityEngine;
namespace GameJam.ExcelMinigame
{
    enum Operations
    {
        Plus,
        Minus,
        Divide,
        Multiply,
        Empty
    }
    public class Calculator : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        TMP_InputField _inputField;

        private float _firstValue;
        private float? _result = null;
        private Operations _operation = Operations.Empty;
        #endregion

        #region Properties
        //Binded to display
        public string ResultString { get; set; } = "0";

        #endregion


        #region Custom Methods
        public void OnNumberClick(int number)
        {
            if (_result is not null || _inputField.text == "0")
            {
                ClearResult();
                ClearEntry();
            }

            AddNumberOnDisplay(number);
        }
        public void OnOperationClick(string operation)
        {
            if (_operation != Operations.Empty)
            {
                float _secondValue = GetValueFromDisplay();
                _firstValue = PerfomOperation(_operation, _firstValue, _secondValue);
            }
            else
                _firstValue = GetValueFromDisplay();

            NullifyEntry();
            _operation = GetOperation(operation);
        }
        public void OnEqualsClick()
        {
            if (_operation == Operations.Empty) return;

            float _secondValue = GetValueFromDisplay();

            _result = PerfomOperation(_operation, _firstValue, _secondValue);

            ShowResult(_result);

            ResetOperation();
        }

        public void OnClearClick()
        {
            _result = 0;
            _firstValue = 0;
            _operation = Operations.Empty;
            NullifyEntry();
        }
        public void OnClearEntryClick()
        {
            NullifyEntry();
        }


        #region Auxiliary Methods
        public void ClearEntry()
        {
            _inputField.text = string.Empty;
        }

        public void NullifyEntry()
        {
            _inputField.text = "0";
        }

        public float GetValueFromDisplay()
        {
            return float.Parse(ResultString);
        }

        private float PerfomOperation(Operations operation, float firstValue, float secondValue)
        {
            return operation switch
            {
                Operations.Plus => firstValue + secondValue,
                Operations.Minus => firstValue - secondValue,
                Operations.Divide => firstValue / secondValue,
                Operations.Multiply => firstValue * secondValue,
                _ => float.NaN,
            };
        }

        private void ShowResult(float? result)
        {
            _inputField.text = result?.ToString();
        }

        private void AddNumberOnDisplay(int number)
        {
            _inputField.text += number.ToString();
        }

        private void ResetOperation()
        {
            _operation = Operations.Empty;
        }

        private Operations GetOperation(string sOperation)
        {
            return sOperation switch
            {
                "+" => Operations.Plus,
                "-" => Operations.Minus,
                "/" => Operations.Divide,
                "*" => Operations.Multiply,
                _ => Operations.Empty
            };
        }

        private void ClearResult()
        {
            _result = null;
        }
        #endregion

        #endregion


    }
}
