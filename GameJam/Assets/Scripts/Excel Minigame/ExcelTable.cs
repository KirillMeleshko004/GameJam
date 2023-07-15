using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameJam.ExcelMinigame
{
    public class ExcelTable : MonoBehaviour, ISerializationCallbackReceiver
    {
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

        public static string InputValue { get; set; }


        [Header("Text file with all table information")]
        [SerializeField]
        private TextAsset _tableInfo;

        [Header("Prefabs for data cells of each column")]
        [SerializeField]
        private GameObject[] _dataCellsPrefabs;



        private Dictionary<int, GameObject> _columnPrefabPairs = new Dictionary<int, GameObject>();

        #region Built-in methods

        private void Start()
        {
            CreateTable();
        }
        #endregion

        #region Custom methods
        private void CreateTable()
        {
            string[] dataCells = (from str in _tableInfo.text.Split() where str != "" select str).ToArray<string>();

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

            Debug.Log(xPos);
            float yPos = _yStart - (_ySpaceBetween + 
                _columnPrefabPairs[columnIndex].GetComponent<RectTransform>().rect.height) * (elInd / (_columnCount));

            return new Vector3(xPos, yPos, 0f);
        }

        public void OnSubmit()
        {
            string[] dataCells = (from str in _tableInfo.text.Split() where str != "" select str).ToArray<string>();
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

            if (Convert.ToInt32(ExcelTable.InputValue) != sum)
                Debug.Log("No");
            else
                Debug.Log("Yes");
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