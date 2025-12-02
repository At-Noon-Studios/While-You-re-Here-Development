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

        private bool _isOpen = false;
        private Quaternion _closeRotation;
        private Quaternion _openRotation;

        protected override void Awake()
        {
            base.Awake();

            if (!doorPivot)
                doorPivot = transform;

            _closeRotation = doorPivot.localRotation;
            _openRotation = Quaternion.Euler(0, config.openAngle, 0) * _closeRotation;

            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        public override void Interact(IInteractor interactor)
        {
            if (keyhole.IsLocked)
            {
                if (audioSource && config.lockedSound)
                    audioSource.PlayOneShot(config.lockedSound);
                return;
            }
            
            _isOpen = !_isOpen;

            if (audioSource)
            {
                if (_isOpen && config.openSound)
                    audioSource.PlayOneShot(config.openSound);
                else if (!_isOpen && config.closeSound)
                    audioSource.PlayOneShot(config.closeSound);
            }
        }

        private void Update()
        {
            float speed = config != null ? config.openSpeed : 2f;

            Quaternion target = _isOpen ? _openRotation : _closeRotation;

            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                target,
                Time.deltaTime * speed
            );
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (keyhole.IsLocked)
                return "Door is locked";

            return _isOpen ? "Press 'E' to Close Door" : "Press 'E' to Open Door";
        }

        public override bool IsInteractableBy(IInteractor interactor) => !keyhole.IsLocked;
        
        public override bool IsDetectableBy(IInteractor interactor) => !keyhole.CurrentlyBeingOperated;
        
    }
}
