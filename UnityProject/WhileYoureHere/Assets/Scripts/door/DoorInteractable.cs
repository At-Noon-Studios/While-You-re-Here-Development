using System.Collections.Generic;
using UnityEngine;
using Interactable;
using ScriptableObjects.door;

namespace door
{
    public class DoorInteractable : InteractableBehaviour
    {

        [Header("Door Config")]
        [SerializeField] private DoorConfig config;

        [Header("References")]
        [SerializeField] private Transform doorPivot;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Keyhole keyhole;

        [SerializeField] public bool isLocked;
        [SerializeField] public bool isOpen;
        
        private Quaternion _closeRotation;
        private Quaternion _openRotation;
        private Transform _playerCamera;

        public bool IsLocked
        {
            set => isLocked = value;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!doorPivot) doorPivot = transform;

            _closeRotation = doorPivot.localRotation;
            _openRotation = Quaternion.Euler(0, config.openAngle, 0) * _closeRotation;

            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null) _playerCamera = cam.transform;
            }
        }

        public bool IsFinishedMoving() => doorPivot.localRotation == (isOpen ? _openRotation : _closeRotation);

        private void Update()
        {
            Quaternion target = isOpen ? _openRotation : _closeRotation;
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, target, Time.deltaTime * (config?.openSpeed ?? 2f));
            
        }
        public override void Interact(IInteractor interactor)
        {
            if (isLocked)
            {
                if (audioSource && config.lockedSound) audioSource.PlayOneShot(config.lockedSound);
                return;
            }
            isOpen = !isOpen;

            if (audioSource)
            {
                if (isOpen && config.openSound) audioSource.PlayOneShot(config.openSound);
                else if (!isOpen && config.closeSound) audioSource.PlayOneShot(config.closeSound);
            }
        }

        public override bool IsInteractableBy(IInteractor interactor) => !isLocked;
        
        public override bool IsDetectableBy(IInteractor interactor) => !keyhole?.CurrentlyBeingOperated ?? base.IsDetectableBy(interactor);
        
        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}
