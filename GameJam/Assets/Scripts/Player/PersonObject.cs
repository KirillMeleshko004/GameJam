using GameJam.Inputs;
using GameJam.ScriptableObjects.Animation;
using UnityEngine;

namespace GameJam.Player
{

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PersonObject : MonoBehaviour
    {
        public Rigidbody2D PersonRb { get; private set; }
        public Animator PersonAnim { get; private set; }
        private PlayerInput PlayerInput { get; set; }
        private ActionController PlayerActionController { get; set; }
        public Transform PersonTransform { get { return transform; } }

        [SerializeField]
        private AnimationsInfo _animationInfo;

        public AnimationsInfo AnimInfo { get {  return _animationInfo; } }


        public void SetAnimTrigger(AnimInfo animInfo)
        {
            if (animInfo == null) return;
            if (animInfo.TriggerType == TriggerTypes.Bool)
                PersonAnim.SetBool(animInfo.TriggerValueName, animInfo.TriggerValue);
            else
                PersonAnim.SetTrigger(animInfo.TriggerValueName);
        }

        public void DisableMovements()
        {
            if(PlayerInput != null)
                PlayerInput.IsMovementEnabled = false;
        }
        public void EnableMovements()
        {
            if (PlayerInput != null)
                PlayerInput.IsMovementEnabled = true;
        }

        public void DisableInteractions()
        {
            if (PlayerActionController != null)
                PlayerActionController.AreInteractionsDisabled = true;
        }
        public void EnableInteractions()
        {
            if (PlayerActionController != null)
                PlayerActionController.AreInteractionsDisabled = false;
        }


        void Start()
        {
            PersonRb = GetComponent<Rigidbody2D>();
            PersonAnim = GetComponent<Animator>();
            PlayerInput = GetComponent<PlayerInput>();
            PlayerActionController = GetComponent<ActionController>();
        }
    }
}