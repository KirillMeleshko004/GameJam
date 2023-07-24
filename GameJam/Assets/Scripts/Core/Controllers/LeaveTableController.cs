using GameJam.Items;
using UnityEngine;

namespace GameJam.Core.Controllers
{
    public class LeaveTableController : MonoBehaviour
    {
        [SerializeField]
        private Table _table;

        public void LeaveTable()
        {
            _table.Interact(gameObject);
        }
    }
}
