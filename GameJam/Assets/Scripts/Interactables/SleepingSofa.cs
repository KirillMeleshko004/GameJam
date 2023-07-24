using GameJam.Core.Interactions;
using GameJam.Core.Movement;
using GameJam.Core.SceneChangers;
using GameJam.Player;
using GameJam.ScriptableObjects.Animation;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Items
{
    public class SleepingSofa : BaseInteractable, IInteractableSceneChanger
    {
        #region Variables
        [Header("Messages, that show up as a hint")]
        [SerializeField]
        private string _sleepHint = "Prees E to sleep";


        [SerializeField]
        private Vector3 _offset = Vector3.zero;

        [SerializeField]
        private int _targetSceneBuildID;

        [SerializeField]
        private Animator _fadeOutAnimator;
        [SerializeField]
        private float _fadeOutAnimTime = 3f;
        #endregion

        #region Custom methods
        private IEnumerator Sleep(PersonObject interactionObject)
        {
            interactionObject.DisableMovements();
            interactionObject.DisableInteractions();

            AnimInfo animInfo = interactionObject.AnimInfo.GetAnimationInfo(AnimationTypes.Sleep);

            Mover.AddMovement(interactionObject.gameObject,
                new Vector3(
                    transform.position.x,
                    interactionObject.PersonTransform.position.y,
                    interactionObject.PersonTransform.position.z
                    ) + _offset
                );

            while (!Mover.IsAtTarget(interactionObject.gameObject))
            {
                yield return new WaitForFixedUpdate();
            }


            interactionObject.SetAnimTrigger(animInfo);
            yield return new WaitForSeconds(animInfo.AnimationLength);

            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene()
        {
            _fadeOutAnimator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(_fadeOutAnimTime);
            SceneManager.LoadScene(_targetSceneBuildID);
        }
        #endregion

        #region IInteractable realisation
        public override GameObject InteractableObject { get { return this.gameObject; } }
        public override void Interact(GameObject sender)
        {
            StartCoroutine(Sleep(sender.GetComponent<PersonObject>()));
        }

        public override void ShowInteractionHint()
        {
            base._hintDisplay.SetActive(true);
            base._hintDisplay.GetComponent<HintDisplay>().DisplayHint(_sleepHint);

        }
        public override void HideInteractionHint()
        {
            base._hintDisplay.GetComponent<HintDisplay>().HideHint();
            base._hintDisplay.SetActive(false);
        }

        #endregion
    }
}
