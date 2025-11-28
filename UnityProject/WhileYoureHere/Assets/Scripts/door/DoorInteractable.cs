using UnityEngine;
using Interactable;
using Interactable.Concrete.Key;

namespace door
{
    public class DoorInteractable : InteractableBehaviour
    {
        [Header("Key Settings")]
        [SerializeField] private Transform keyHolePosition;
        [SerializeField] private Vector3 keyRotation;
        
        [Header("Door Settings")]
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openSpeed = 3f;
        [SerializeField] private Transform doorPivot;

        [Header("Door Sounds")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private AudioClip lockedSound;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Door Lock")]
        [SerializeField] private bool isLocked = false;

        private bool _isOpen = false;
        private Quaternion _closeRotation;
        private Quaternion _openRotation;

        protected override void Awake()
        {
            base.Awake();

            if (!doorPivot)
                doorPivot = transform;

            _closeRotation = doorPivot.localRotation;
            _openRotation = Quaternion.Euler(0, openAngle, 0) * _closeRotation;

            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject is Key key)
            {
                key.Place(keyHolePosition.position, Quaternion.Euler(keyRotation));
                key.StartMinigame(interactor, (newState) => isLocked = newState);
                return;
            }
            if (isLocked)
            {
                if (audioSource && lockedSound)
                    audioSource.PlayOneShot(lockedSound);
                return;
            }

            _isOpen = !_isOpen;

            if (audioSource)
            {
                if (_isOpen && openSound)
                    audioSource.PlayOneShot(openSound);
                else if (!_isOpen && closeSound)
                    audioSource.PlayOneShot(closeSound);
            }
        }

        private void Update()
        {
            Quaternion target = _isOpen ? _openRotation : _closeRotation;

            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                target,
                Time.deltaTime * openSpeed
            );
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (isLocked)
                return "Door is locked";

            return _isOpen ? "Press 'E' to Close Door" : "Press 'E' to Open Door";
        }
    }
}
