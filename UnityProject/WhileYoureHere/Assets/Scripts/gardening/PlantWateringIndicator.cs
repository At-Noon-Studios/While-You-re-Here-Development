using System.Collections;
using Interactable;
using UnityEngine;
using UnityEngine.UI;

namespace gardening
{
    public class PlantWateringIndicator : InteractableBehaviour
    {
        [Header("References")] [SerializeField]
        private PlantObject plant;

        [SerializeField] private Image dryImage;
        [SerializeField] private Image wateringImage;
        [SerializeField] private Image failedImage;

        private Transform _playerCamera;
        private Coroutine _updateCoroutine;

        protected override void Awake()
        {
            base.Awake();
            
            if (dryImage != null) dryImage.enabled = false;
            if (wateringImage != null) wateringImage.enabled = false;
            if (failedImage != null) failedImage.enabled = false;
        }
        private void Update()
        {
            if (_playerCamera != null)
            {
                if (dryImage != null && dryImage.enabled) dryImage.transform.LookAt(_playerCamera);
                if (wateringImage != null && wateringImage.enabled) wateringImage.transform.LookAt(_playerCamera);
                if (failedImage != null && failedImage.enabled) failedImage.transform.LookAt(_playerCamera);
            }
        }

        private void UpdateImageVisibility()
        {
            if (plant == null || plant.PlantData == null)
                return;

            var stage = plant.CurrentStage;
            var max = plant.PlantData.MaxStage;

            dryImage.enabled = (stage == 0);
            wateringImage.enabled = (stage > 0 && stage < max - 1);
            failedImage.enabled = (stage >= max - 1);
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            // Start the coroutine that continuously checks the visibility
            if (_updateCoroutine == null)
            {
                _updateCoroutine = StartCoroutine(UpdateImagesCoroutine());
            }
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            if (_updateCoroutine != null)
            {
                StopCoroutine(_updateCoroutine); // Stop the updating coroutine
                _updateCoroutine = null; // Reset the coroutine reference
            }

            // Disable images when not hovering
            if (dryImage != null) dryImage.enabled = false;
            if (wateringImage != null) wateringImage.enabled = false;
            if (failedImage != null) failedImage.enabled = false;
        }

        private IEnumerator UpdateImagesCoroutine()
        {
            while (true)
            {
                UpdateImageVisibility(); // Update visibility each frame
                yield return null; // Wait for the next frame
            }
        }
        
        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
        
        public override void Interact(IInteractor interactor) {}
    }
}
