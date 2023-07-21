using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.ScriptableObjects.Animation
{
    public enum AnimationTypes
    {
        Idle,
        Walk,
        StandUpFromSofa,
        SitDownToSofa,
        StandUpFromChair,
        SitDownToChair,
        Shoot,
        TakeCoffee,
        DrinkCoffee,
        LeaveTable,
        Sleep
    }
    public enum TriggerTypes
    {
        Trigger,
        Bool
    }

    [CreateAssetMenu(fileName = "New Animation Info", menuName = "Animation Info")]
    public class AnimationsInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<AnimInfo> _animations;

        private readonly Dictionary<AnimationTypes, AnimInfo> _animationsDict = new();


        public AnimInfo GetAnimationInfo(AnimationTypes type)
        {
            return _animationsDict[type];
        }

        public void OnAfterDeserialize()
        {
            _animationsDict.Clear();
            foreach (var animation in _animations) 
                _animationsDict.TryAdd(animation.AnimationType, animation);
        }

        public void OnBeforeSerialize()
        {
            //No need
        }
    }

    [Serializable]
    public class AnimInfo
    {
        [SerializeField]
        private AnimationTypes _animationType;
        [SerializeField]
        private TriggerTypes _triggerType;
        [SerializeField]
        private string _triggerValueName;
        [SerializeField]
        private bool _triggerValue = true;
        [SerializeField]
        private float _animationLength;

        public AnimationTypes AnimationType { get { return _animationType; } }
        public TriggerTypes TriggerType { get { return _triggerType; } }
        public string TriggerValueName { get { return _triggerValueName; } }
        public bool TriggerValue { get { return _triggerValue; } }
        public float AnimationLength { get { return _animationLength; } }
    }
}
