using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Player
{
    public class Suicide : MonoBehaviour
    {
        [SerializeField]
        private Animator _shootScreenAnim;
        [SerializeField]
        private string _shootTriggerName;
        [SerializeField]
        private float _shootAnimTime;
        [SerializeField]
        private float _takePistolAnimTime;
        [SerializeField]
        private string _takePistolAnimBoolName;

        [SerializeField]
        private int _finalScenBuildId;
        public void ShootDown()
        {
            StartCoroutine(TakePistol());
        }

        private IEnumerator TakePistol()
        {
            Animator playerAnim = gameObject.GetComponent<Animator>();
            playerAnim.SetBool(_takePistolAnimBoolName, true);
            yield return new WaitForSeconds(_takePistolAnimTime);
            StartCoroutine(Shoot());
        }
        private IEnumerator Shoot()
        {
            _shootScreenAnim.SetTrigger(_shootTriggerName);
            yield return new WaitForSeconds(_shootAnimTime);
            SceneManager.LoadScene(_finalScenBuildId);
        }
    }
}
