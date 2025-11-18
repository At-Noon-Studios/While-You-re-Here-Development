using System;
using System.Linq;
using UI;
using UnityEngine;

namespace Interactable
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))] //Not used in this script, but if you add InteractableBehaviour to something that doesn't have a collider you will never receive an 'Interact' callback.
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
        
        public virtual void OnHoverEnter()
        {
            print("base hover enter");
            AddOutlineMaterialToRenderer();
            _uiManager?.ShowInteractPrompt(gameObject.name);
        }

        public virtual void OnHoverExit()
        {
            print("base hover exit");
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