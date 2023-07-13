using TMPro;
using UnityEngine;

namespace GameJam.Core.Interactions
{
    //Class represent display for hint messages
    public class HintDisplay : MonoBehaviour
    {
        #region Variables
        #endregion


        #region Built-in methods
        #endregion

        #region Custom methods
        public void DisplayHint(string hint)
        {
            TextMeshProUGUI textMesh = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            textMesh.text = hint;
        }
        public void HideHint()
        {
            TextMeshProUGUI textMesh = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            textMesh.text = "";
        }
        #endregion
    }
}

