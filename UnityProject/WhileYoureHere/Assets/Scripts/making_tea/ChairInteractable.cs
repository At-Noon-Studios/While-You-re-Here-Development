using Interactable;
using player_controls;
using UnityEngine;

namespace making_tea
{
    public class ChairInteractable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private Transform sitPoint;
        [SerializeField] private Transform lookTarget;

        private bool _isSitting;

        private PlayerInteractionController _pic;
        private MovementController _movement;
        private CameraController _cameraController;
        private Transform _player;
        private Camera _playerCam;

        public bool InteractableBy(IInteractor interactor)
        {
            return true;
        }

        public string InteractionText(IInteractor interactor)
        {
            return _isSitting ? "[E] Stand up" : "[E] Sit";
        }

        public void EnableCollider(bool enable)
        {
            var col = GetComponent<Collider>();
            if (col != null)
                col.enabled = enable;
        }

        public void OnHoverEnter(IInteractor interactor) { }

        public void OnHoverExit(IInteractor interactor) { }

        public void Interact(IInteractor interactor)
        {
            if (!_isSitting)
                Sit(interactor);
            else
                StandUp();
        }

        private void Sit(IInteractor interactor)
        {
            var p = interactor as PlayerInteractionController;
            if (p == null)
            {
                Debug.LogWarning("ChairInteractable: Interactor is not a PlayerInteractionController!");
                return;
            }

            _player = p.transform;
            _movement = p.GetComponent<MovementController>();
            _playerCam = p.GetComponentInChildren<Camera>();
            _cameraController = p.GetComponentInChildren<CameraController>();

            if (_movement != null) _movement.enabled = false;
            if (_cameraController != null) _cameraController.enabled = false;

            _player.position = sitPoint.position;
            _player.rotation = sitPoint.rotation;

            if (lookTarget != null && _playerCam != null)
            {
                var dir = lookTarget.position - _playerCam.transform.position;
                _playerCam.transform.rotation = Quaternion.LookRotation(dir);
            }

            _isSitting = true;

            _pic = p;
            _pic.EnableTableMode(true);
            _pic.SetSittingChair(this);
        }

        private void StandUp()
        {
            if (_movement != null) _movement.enabled = true;
            if (_cameraController != null) _cameraController.enabled = true;

            _isSitting = false;

            if (_pic == null) return;
            _pic.EnableTableMode(false);
            _pic.ClearSittingChair();
        }
        
        public void ForceStandUp()
        {
            if (_isSitting)
                StandUp();
        }
    }
}
