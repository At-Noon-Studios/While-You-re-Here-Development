using Interactable;
using UnityEngine;
using UnityEngine.UI;

namespace making_tea
{
    public class WaterTapInteractable : InteractableBehaviour
    {
        [Header("Water Tap Reference")]
        [Header("Interaction UI")]
        [SerializeField] private Image tapOnImage;
        [SerializeField] private Image tapOffImage;
        
        public WaterTap tap;
        private Transform _playerCamera;
        
        protected override void Awake()
        {
            base.Awake();

            if (tapOnImage != null) tapOnImage.enabled = false;
            if (tapOffImage != null) tapOffImage.enabled = false;
        }
        
        private void Update()
        {
            if (_playerCamera != null)
            {
               if (tapOnImage != null && tapOnImage.enabled) tapOnImage.transform.LookAt(_playerCamera);
               if (tapOffImage != null && tapOffImage.enabled) tapOffImage.transform.LookAt(_playerCamera);
            }
        }
        
        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            if (!tap.IsRunning && tapOnImage != null)
            {
                tapOnImage.enabled = true;
                tapOffImage.enabled = false;
            }
            else
            {
                tapOffImage.enabled = true;
                tapOnImage.enabled = false;
            }
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (tapOnImage != null)
                tapOnImage.enabled = false;
            if (tapOffImage != null)
                tapOffImage.enabled = false;
        }
        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }

        public override void Interact(IInteractor interactor)
        {
            tap.ToggleTap();
        }
    }
}