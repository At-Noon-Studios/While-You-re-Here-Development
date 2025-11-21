namespace Interactable
{
    internal interface IInteractable
    {
        /// <summary>
        /// Interact is called when a <see cref="PlayerControls.PlayerInteractionController"/> 
        /// triggers an interaction while the player is detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        public void Interact();
        
        /// <summary>
        /// OnHoverEnter is called when a <see cref="PlayerControls.PlayerInteractionController"/> 
        /// starts detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        public void OnHoverEnter();
        
        /// <summary>
        /// OnHoverExit is called when a <see cref="PlayerControls.PlayerInteractionController"/> 
        /// stops detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        public void OnHoverExit();
    }
}