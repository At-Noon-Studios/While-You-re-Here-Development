using Interactable;
using player_controls;
using PlayerControls;
using UnityEngine;

namespace EndDay
{
    public class EndDayInteractable : InteractableBehaviour
    {
        [Header("End day screen settings")]
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayImageDuration = 1f;
        [SerializeField] private CanvasGroup endDayCanvasGroup;
        
        private float _timer;
        private bool _isEndDay = false;
        private CameraController _cameraController;
        private MovementController _movementController;

        protected override void Awake()
        {
            base.Awake();
            _cameraController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CameraController>();
            _movementController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MovementController>();
        }
        
        public override void Interact(IInteractor interactor)
        {
            _isEndDay = true;
            _cameraController.PauseCameraMovement();
            _movementController.PauseMovement();
            blockInteraction = true;
        }

        private void Update()
        {
            if (_isEndDay) EndDay();
        }

        private void EndDay()
        {
            endDayCanvasGroup.alpha = _timer / fadeDuration;
            _timer += Time.deltaTime;

            if (_timer > fadeDuration + displayImageDuration)
            {
                // quit game (or maybe change scene) can be added here 
            }
        }
    }
}