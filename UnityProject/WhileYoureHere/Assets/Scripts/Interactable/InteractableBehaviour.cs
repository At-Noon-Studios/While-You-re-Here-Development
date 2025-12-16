using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// A generic implementation of <see cref="IInteractable"></see>. A script should inherit from this class if you wish to create an interactable that already implements some standard visual feedback.
    /// </summary>
    [RequireComponent(typeof(Collider))] // Not used in this script, but if you add InteractableBehaviour to something that doesn't have a collider you will never receive an 'Interact' callback.
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        private Collider _collider;
        private Renderer[] _renderers;
        private Material _outlineMaterial;
        private const string OutlineMaterialResourcePath = "OutlineMaterial";
        private int _originalLayer;

        #region Unity event functions
        
        /// <remarks>
        /// Be sure to call <c>base.Awake();</c> when overriding this method. Not doing so will prevent outlines from being rendered.
        /// </remarks>
        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
            if (_collider == null) Debug.LogError("Scene contains an InteractableBehaviour that doesn't have a collider.");
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers == null || _renderers.Length == 0) Debug.LogWarning("Scene contains an InteractableBehaviour without any renderers.");
            _outlineMaterial = Resources.Load<Material>(OutlineMaterialResourcePath);
            _originalLayer = gameObject.layer; // this should probably be in start, but I added it here to avoid issues with forgetting to call base.Start
        }
        
        #endregion
        
        #region Interface implementation
        
        public virtual bool InteractableBy(IInteractor interactor) => true;

        public abstract void Interact(IInteractor interactor);
        
        public virtual void OnHoverEnter(IInteractor interactor)
        {
            gameObject.layer = LayerMask.NameToLayer("Outline");
        }
        
        public virtual void OnHoverExit(IInteractor interactor)
        {
            gameObject.layer = _originalLayer;
        }
        
        public virtual string InteractionText(IInteractor interactor) => gameObject.name;
        
        public void EnableCollider(bool state)
        {
            _collider.enabled = state;
        }
        
        #endregion
    }
}