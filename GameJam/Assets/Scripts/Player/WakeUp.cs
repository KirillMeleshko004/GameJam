using GameJam.ScriptableObjects.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Player
{
    [RequireComponent(typeof(PersonObject))]
    public class WakeUp : MonoBehaviour
    {
        private PersonObject _player;

        
        void Start()
        { 
            _player = GetComponent<PersonObject>();
            StartCoroutine(StandUp());
        }

        private IEnumerator StandUp()
        {
            _player.DisableMovements();
            _player.DisableInteractions();

            AnimInfo animInfo = _player.AnimInfo.GetAnimationInfo(AnimationTypes.WakeUp);
            _player.SetAnimTrigger(animInfo);

            yield return new WaitForSeconds(animInfo.AnimationLength);

            _player.EnableMovements();
            _player.EnableInteractions();
        }
    }
}
