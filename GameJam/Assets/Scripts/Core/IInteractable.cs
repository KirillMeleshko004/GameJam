namespace GameJam.Core
{
    //Interface indicates, that interaction with object is possible
    public interface IInteractable
    {
        //Method to interact with object
        public void Interact();

        //Methods to show interaction hint, when interaction is possible
        public void ShowInteractionHint();
        public void HideInteractionHint();
    }
}
