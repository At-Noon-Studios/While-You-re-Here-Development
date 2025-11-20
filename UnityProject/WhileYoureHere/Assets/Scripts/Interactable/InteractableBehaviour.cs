using System.Linq;
using UI;
using UnityEngine;

namespace Interactable
{
    [RequireComponent(typeof(Collider))] //Not used in this script, but if you add InteractableBehaviour to something that doesn't have a collider you will never receive an 'Interact' callback.
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        private Material _outlineMaterial;
        private UIManager _uiManager;
        private Renderer[] _renderers;

        private const string OutlineMaterialResourcePath = "OutlineMaterial";

        protected void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers == null || _renderers.Length == 0) Debug.LogWarning("Scene contains an InteractableBehaviour without any renderers.");
            _outlineMaterial = Resources.Load<Material>(OutlineMaterialResourcePath);
            _uiManager = UIManager.Instance;
        }
        
        public abstract void Interact();
        
        public virtual void OnHoverEnter()
        {
            AddOutlineMaterialToRenderers();
            _uiManager?.ShowInteractPrompt(gameObject.name);
        }

        public virtual void OnHoverExit()
        {
            RemoveOutlineMaterialFromRenderers();
            _uiManager?.HideInteractPrompt();
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