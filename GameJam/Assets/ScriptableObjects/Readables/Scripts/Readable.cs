using UnityEngine;

namespace ScriptableObjects.Readables
{
    //Represent text block to display
    [CreateAssetMenu(fileName = "New Readable", menuName = "Readable")]
    public class Readable : ScriptableObject
    {
        #region Variables
        [SerializeField]
        private string _text;
        [SerializeField]
        private Sprite _background;
        #endregion

        #region Properties
        public string Text { get { return _text; } }
        public Sprite Background { get { return _background; } }
        #endregion
    }
}
