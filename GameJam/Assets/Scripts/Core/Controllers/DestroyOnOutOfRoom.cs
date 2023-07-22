using GameJam.Core.Movement;
using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class DestroyOnOutOfRoom : MonoBehaviour
    {
        void Update()
        {
            if(Mover.IsAtTarget(gameObject))
                gameObject.SetActive(false);
        }
    }
}
