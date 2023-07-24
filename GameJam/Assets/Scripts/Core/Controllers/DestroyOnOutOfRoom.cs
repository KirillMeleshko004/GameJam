using GameJam.Core.Movement;
using System.Collections;
using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class DestroyOnOutOfRoom : MonoBehaviour
    {
        void LateUpdate()
        {
            StartCoroutine(WaitBeforeStart());
        }

        private IEnumerator WaitBeforeStart()
        {

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            if (Mover.IsAtTarget(gameObject))
                gameObject.SetActive(false);
        }
    }
}
