using System;
using UI;
using UnityEngine;

namespace Interactable
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))] //Not used in this script but it also makes no sense to add this component without having a collider
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private UIManager uiManager;
        private Renderer _renderer;
    
        protected virtual void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public abstract void Interact();

        void IInteractable.OnHoverEnter()
        {
            AddOutlineMaterialToRenderer();
            uiManager.ShowInteractPrompt(gameObject.name);
        }

        void IInteractable.OnHoverExit()
        {
            RemoveOutlineMaterialFromRenderer();
            uiManager.HideInteractPrompt();
        }
        
        private void AddOutlineMaterialToRenderer()
        {
            var newMaterials = new Material[_renderer.materials.Length + 1];
            _renderer.materials.CopyTo(newMaterials, 0);
            newMaterials[^1] = outlineMaterial;
            _renderer.materials = newMaterials;
        }

        private void RemoveOutlineMaterialFromRenderer()
        {
            var newMaterials = new Material[_renderer.materials.Length - 1];
            Array.Copy(_renderer.materials, newMaterials, newMaterials.Length);
            _renderer.materials = newMaterials;
        }
    }
}