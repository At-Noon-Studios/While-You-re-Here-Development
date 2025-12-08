using System.Collections.Generic;
using UnityEngine;
using Interactable;
using ScriptableObjects.door;

namespace door
{
    public class DoorInteractable : InteractableBehaviour
    {
        [Header("Interaction Canvases (0=open front, 1=open back, 2=close front, 3=close back)")]
        [SerializeField] private List<Canvas> interactionCanvases;

        [Header("Door Config")]
        [SerializeField] private DoorConfig config;

        [Header("References")]
        [SerializeField] private Transform doorPivot;
        [SerializeField] private AudioSource audioSource;

        private bool _isLocked;
        private bool _isOpen = false;
        private Quaternion _closeRotation;
        private Quaternion _openRotation;
        private Transform _playerCamera;
        
        public void LockDoor(bool locked) { _isLocked = locked; }

        protected new void Awake()
        {
            base.Awake();
            _isLocked = config.isLocked;
            if (!doorPivot) doorPivot = transform;

            _closeRotation = doorPivot.localRotation;
            _openRotation = Quaternion.Euler(0, config.openAngle, 0) * _closeRotation;

            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

            if (interactionCanvases != null)
                interactionCanvases.ForEach(c => c.gameObject.SetActive(false));

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null) _playerCamera = cam.transform;
            }
        }

        private void Update()
        {
            Quaternion target = _isOpen ? _openRotation : _closeRotation;
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, target, Time.deltaTime * (config?.openSpeed ?? 2f));

            if (_playerCamera != null && interactionCanvases != null)
            {
                interactionCanvases.ForEach(c =>
                {
                    if (c.gameObject.activeSelf)
                    {
                        c.transform.LookAt(_playerCamera);
                        c.transform.Rotate(0f, 180f, 0f);
                    }
                });
            }
        }

        public override void Interact(IInteractor interactor)
        {
            if (_isLocked)
            {
                if (audioSource && config.lockedSound) audioSource.PlayOneShot(config.lockedSound);
                return;
            }

            _isOpen = !_isOpen;

            if (audioSource)
            {
                if (_isOpen && config.openSound) audioSource.PlayOneShot(config.openSound);
                else if (!_isOpen && config.closeSound) audioSource.PlayOneShot(config.closeSound);
            }
        }

        public override void ClickInteract(IInteractor interactor)
        {
            throw new System.NotImplementedException();
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);
            if (_playerCamera == null || interactionCanvases == null || _isLocked) return;

            bool isFront = Vector3.Dot(doorPivot.forward, (_playerCamera.position - doorPivot.position).normalized) > 0f;

            interactionCanvases.ForEach(c => c.gameObject.SetActive(false));

            int index = !_isOpen ? (isFront ? 0 : 1) : (isFront ? 2 : 3);
            if (index < interactionCanvases.Count)
                interactionCanvases[index].gameObject.SetActive(true);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);
            if (interactionCanvases == null) return;
            interactionCanvases.ForEach(c => c.gameObject.SetActive(false));
        }

        public override string InteractionText(IInteractor interactor) => string.Empty;
    }
}
