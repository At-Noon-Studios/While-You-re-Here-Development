namespace Interactable
{
    public interface IInteractable
    {
        /// <summary>
        /// Determines the text that will be used when rendering the interaction prompt when looking at interactable objects.
        /// <returns>A string that will be used to indicate what you are interacting with.</returns>
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public string InteractionText(IInteractor interactor);
        
        /// <summary>
        /// Determines whether <paramref name="interactor"/> is allowed to interact with this <see cref="IInteractable"/> component.
        /// <returns>A bool that is used to determine whether an interaction can occur.</returns>
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public bool InteractableBy(IInteractor interactor);
     
        /// <summary>
        /// Determines whether <paramref name="interactor"/> is able to detect this <see cref="IInteractable"/> component.
        /// <returns>A bool that is used to determine whether this instance is detectable.</returns>
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public bool DetectableBy(IInteractor interactor);
        
        /// <summary>
        /// Called when a <see cref="PlayerInteractionController"/> 
        /// triggers an interaction while the player is detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public void Interact(IInteractor interactor);
        
        /// <summary>
        /// OnHoverEnter is called when a <see cref="PlayerInteractionController"/> 
        /// starts detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public void OnHoverEnter(IInteractor interactor);
        
        /// <summary>
        /// OnHoverExit is called when a <see cref="PlayerInteractionController"/> 
        /// stops detecting a Collider containing 
        /// this <see cref="IInteractable"/> component.
        /// </summary>
        /// <param name="interactor">
        /// The <see cref="IInteractor"/> that is performing the interaction.
        /// </param>
        public void OnHoverExit(IInteractor interactor);

        /// <summary>
        /// This method sets the collider state of this <see cref="IInteractable"/> component.
        /// </summary>
        /// <param name="state">
        /// The new collider state. When state is set to true, there should be an active collider.
        /// </param>
        public void EnableCollider(bool state);
    }
}