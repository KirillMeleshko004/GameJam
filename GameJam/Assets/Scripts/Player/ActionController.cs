using GameJam.Inputs;
using UnityEngine;

namespace GameJam.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionController : MonoBehaviour
    {
        #region Variables
        private PlayerInput _playerRInput;
        #endregion

        #region Properties

        #endregion


        #region Built-in methods
        private void Start()
        {
            _playerRInput = GetComponent<PlayerInput>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Trigger");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log("Exit trigger");
        }
        #endregion


        #region Custom methods
        #endregion
    }
}
