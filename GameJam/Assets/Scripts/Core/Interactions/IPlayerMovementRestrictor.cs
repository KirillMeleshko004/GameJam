using GameJam.Inputs;
using UnityEngine;

namespace GameJam.Core.Interactions
{
    public interface IPlayerMovementRestrictor
    {
        public void DisablePlayerMovement(PlayerInput input);
        public void EnablePlayerMovement(PlayerInput input);
    }
}
