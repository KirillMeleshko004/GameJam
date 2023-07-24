using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam.ExcelMinigame
{
    public class ExcelTableGame : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Events
        public event Action GameCompleted;
        #endregion

        #region Variables
        [Header("Messages Values")]
        [SerializeField]
        private string _correctMessage;
        [SerializeField]
        private string _incorrectMessage;
        [Header("Messages Box Background Colors")]
        [SerializeField]
        private Color _correctColor;
        [SerializeField]
        private Color _incorrectColor;

        [SerializeField]
        private float _xStart;
        [SerializeField]
        private float _yStart;
        [SerializeField]
        private float _xSpaceBetween;
        [SerializeField]
        private float _ySpaceBetween;
        [SerializeField]
        private int _columnCount;

        [SerializeField]
        private GameObject _resultBox;
        #endregion

        public string InputValue { get; set; } = string.Empty;


        [Header("Text file with all table information")]
        [SerializeField]
        private TextAsset _tableInfo;

        [Header("Prefabs for data cells of each column")]
        [SerializeField]
        private GameObject[] _dataCellsPrefabs;

        private Dictionary<int, GameObject> _columnPrefabPairs = new Dictionary<int, GameObject>();

        private string[] dataCells;

        #region Built-in methods

        private void Start()
        {
            dataCells = (from str in _tableInfo.text.Split('|') where str.Trim() != "" select str.Trim()).ToArray<string>();

            CreateTable();
        }
        #endregion

        #region Custom methods

        private void CreateTable()
        {
            for (int noteInd = 0; noteInd < dataCells.Length; noteInd += _columnCount)
            {
                for (int i = 0; i < _columnCount; i++)
                {
                    GameObject cell = Instantiate(_columnPrefabPairs[i], Vector3.zero, Quaternion.identity, gameObject.transform);
                    cell.GetComponentInChildren<TextMeshProUGUI>().text = dataCells[noteInd + i];
                    cell.transform.localPosition = GetPosition(noteInd + i, i);
                }
            }
        }


        private Vector3 GetPosition(int elInd, int columnIndex)
        {
            float xPos = _columnPrefabPairs[columnIndex].transform.localPosition.x;

            float yPos = _yStart - (_ySpaceBetween + 
                _columnPrefabPairs[columnIndex].GetComponent<RectTransform>().rect.height) * (elInd / (_columnCount));

            return new Vector3(xPos, yPos, 0f);
        }

        public void OnSubmit()
        {
            if (InputValue == string.Empty) return;

            int sum = 0;
            for (int i = 2; i < dataCells.Length; i += 3)
            {
                string numericString = String.Empty;
                foreach (var c in dataCells[i])
                {
                    // Check for numeric characters (hex in this case) or leading or trailing spaces.
                    if ((c >= '0' && c <= '9'))
                    {
                        numericString = string.Concat(numericString, c.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
                sum += int.Parse(numericString);

            }

            ShowResult(Convert.ToInt32(InputValue) == sum);

        }

        private void ShowResult(bool isCorrect)
        {
            _resultBox.SetActive(true);

            TextMeshProUGUI textBox = _resultBox.GetComponentInChildren<TextMeshProUGUI>();
            Image backgroundImage = _resultBox.GetComponent<Image>();
            if(isCorrect)
            {
                textBox.text = _correctMessage;
                backgroundImage.color = _correctColor;
                GameCompleted?.Invoke();
            }
            else
            {
                textBox.text = _incorrectMessage;
                backgroundImage.color = _incorrectColor;
            }
        }

        #endregion

        #region ISerializationCallbackReceiver implementation
        public void OnAfterDeserialize()
        {
            _columnPrefabPairs = new Dictionary<int, GameObject>();
            for (int i = 0; i < _dataCellsPrefabs.Length; i++)
            {
                _columnPrefabPairs.Add(i, _dataCellsPrefabs[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            //no need
        }
        #endregion
    }
}