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
        public PlayerInput PlayerInput { get; private set; }
        public Transform PersonTransform { get { return transform; } }

        [SerializeField]
        private AnimationsInfo _animationInfo;

        public AnimationsInfo AnimInfo { get {  return _animationInfo; } }


        void Start()
        {
            PersonRb = GetComponent<Rigidbody2D>();
            PersonAnim = GetComponent<Animator>();
            PlayerInput = GetComponent<PlayerInput>();
        }
    }
}