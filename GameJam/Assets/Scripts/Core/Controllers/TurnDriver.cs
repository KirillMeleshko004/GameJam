using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class TurnDriver : MonoBehaviour
    {
        public void TurnAround()
        {
            GetComponent<Animator>().SetTrigger("turn");
        }
    }
}
