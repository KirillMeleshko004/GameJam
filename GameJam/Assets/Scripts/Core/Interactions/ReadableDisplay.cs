using GameJam.Player;
using ScriptableObjects.Readables;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam.Core.Interactions
{
    //Class represent display for readable objects
    public class ReadableDisplay : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private GameObject _readableDisplayPrefab;
        [SerializeField]
        private GameObject _player;
        #endregion


        #region Built-in methods
        #endregion

        #region Custom methods
        public void DisplayReadable(Readable readable)
        {
            GameObject obj = Instantiate(_readableDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            obj.GetComponent<Image>().sprite = readable.Background;
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = readable.Text;

            _player.GetComponent<MovementController>().enabled = false;
        }
        public void HideReadable()
        {
            GameObject.Destroy(transform.GetChild(0).gameObject);
            _player.GetComponent<MovementController>().enabled = true;
        }
        #endregion
    }
}
