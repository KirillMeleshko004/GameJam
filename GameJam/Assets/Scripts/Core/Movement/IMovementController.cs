namespace GameJam.Core.Movement
{

    public interface IMovementController
    {
        public float MaxSpeed { get; }
        public string AnimatorIsMovingBoolName { get; }
    }
}
