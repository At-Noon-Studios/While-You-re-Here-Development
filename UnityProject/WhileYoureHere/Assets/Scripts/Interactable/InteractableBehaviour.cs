using System.Linq;
using UI;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// A generic implementation of <see cref="IInteractable"></see>. A script should inherit from this class if you wish to create an interactable that already implements some standard visual feedback.
    /// </summary>
    [RequireComponent(typeof(Collider))] //Not used in this script, but if you add InteractableBehaviour to something that doesn't have a collider you will never receive an 'Interact' callback.
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        private Material _outlineMaterial;
        private UIManager _uiManager;
        private Renderer[] _renderers;

        private const string OutlineMaterialResourcePath = "OutlineMaterial";

        /// <remarks>
        /// Be sure to call <c>base.Awake();</c> when overriding this function. Not doing so will prevent outlines from being rendered.
        /// </remarks>
        protected void Awake()
        {
            if (GetComponent<Collider>() == null) Debug.LogError("Scene contains an InteractableBehaviour that doesn't have a collider.");
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers == null || _renderers.Length == 0) Debug.LogWarning("Scene contains an InteractableBehaviour without any renderers.");
            _outlineMaterial = Resources.Load<Material>(OutlineMaterialResourcePath);
        }

        /// <remarks>
        /// Be sure to call <c>base.Start();</c> when overriding this function. Not doing so will prevent the default interaction prompt from showing up.
        /// </remarks>
        protected void Start()
        {
            _uiManager = UIManager.Instance;
        }
        
        public abstract void Interact();
        
        public virtual void OnHoverEnter()
        {
            AddOutlineMaterialToRenderers();
            _uiManager?.ShowInteractPrompt(InteractionText());
        }
        
        public virtual void OnHoverExit()
        {
            RemoveOutlineMaterialFromRenderers();
            _uiManager?.HideInteractPrompt();
        }
        
        /// <summary>
        /// <returns>A string that will be used to indicate what you are interacting with.</returns>> Determines the text that will be used when rendering the interaction prompt when looking at interactable objects.
        /// </summary>
        protected virtual string InteractionText()
        {
            return gameObject.name;
        }
        
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
    }
}