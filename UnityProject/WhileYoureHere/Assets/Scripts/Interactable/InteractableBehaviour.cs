using System;
using System.Linq;
using UI;
using UnityEngine;

namespace Interactable
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))] //Not used in this script, but it also makes no sense to add this component without having a collider
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        private Material _outlineMaterial;
        private UIManager _uiManager;
        private Renderer _renderer;

        private const string OutlineMaterialResourcePath = "OutlineMaterial";

        protected void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _outlineMaterial = Resources.Load<Material>(OutlineMaterialResourcePath);
            _uiManager = UIManager.Instance;
        }
        
        public abstract void Interact();
        
        void IInteractable.OnHoverEnter()
        {
            AddOutlineMaterialToRenderer();
            _uiManager?.ShowInteractPrompt(gameObject.name);
        }

        void IInteractable.OnHoverExit()
        {
            RemoveOutlineMaterialFromRenderer();
            _uiManager?.HideInteractPrompt();
        }
        
        private void AddOutlineMaterialToRenderer()
        {
            if (!_renderer || !_outlineMaterial) return;
            var materials = _renderer.materials;
            if (materials.Contains(_outlineMaterial)) return;
            _renderer.materials = materials.Append(_outlineMaterial).ToArray();
        }

        private void RemoveOutlineMaterialFromRenderer() {
            if (!_renderer || !_outlineMaterial) return;
            var materials = _renderer.materials;
            if (!materials.Contains(_outlineMaterial)) return;
            _renderer.materials = materials.Where(m => !m.Equals(_outlineMaterial)).ToArray();
        }
    }
}