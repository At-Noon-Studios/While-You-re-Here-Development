using System.Linq;
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
        }
        
        #endregion
        
        #region Interface implementation
        
        public virtual bool InteractableBy(IInteractor interactor) => true;

        public abstract void Interact(IInteractor interactor);
        
        public virtual void OnHoverEnter(IInteractor interactor)
        {
            AddOutlineMaterialToRenderers();
        }
        
        public virtual void OnHoverExit(IInteractor interactor)
        {
            RemoveOutlineMaterialFromRenderers();
        }
        
        public virtual string InteractionText(IInteractor interactor) => gameObject.name;
        
        public void EnableCollider(bool state)
        {
            _collider.enabled = state;
        }
        
        #endregion
        
        #region Private methods
        
        private void AddOutlineMaterialToRenderers()
        {
            foreach (var rendererComponent in _renderers)
            {
                AddOutlineMaterialToRenderer(rendererComponent);
            }
        }

        private void AddOutlineMaterialToRenderer(Renderer rendererComponent)
        {
            if (!rendererComponent || !_outlineMaterial) return;
            var materials = rendererComponent.materials;
            if (materials.Any(m => m.name.StartsWith(_outlineMaterial.name)))
                return;
            rendererComponent.materials = materials.Append(_outlineMaterial).ToArray();
        }

        private void RemoveOutlineMaterialFromRenderers()
        {
            foreach (var rendererComponent in _renderers)
            {
                RemoveOutlineMaterialFromRenderer(rendererComponent);
            }
        }

        private void RemoveOutlineMaterialFromRenderer(Renderer rendererComponent)
        {
            if (!rendererComponent || !_outlineMaterial) return;
            var materials = rendererComponent.materials;
            if (!materials.Any(m => m.name.StartsWith(_outlineMaterial.name)))
                return;
            rendererComponent.materials = materials.Where(m => !m.name.StartsWith(_outlineMaterial.name)).ToArray();
        }
        
        #endregion
    }
}