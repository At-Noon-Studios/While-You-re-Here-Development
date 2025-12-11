using chore;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace entity
{
    public class WateringCan : MonoBehaviour
    {
        [Header("WateringCan settings")]
        [SerializeField] private int wateringCanID;
        [SerializeField] private Transform rotationRoot;

        [Header("Sound")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickUpSound;
        [SerializeField] private AudioClip dropSound;

        private Transform _playerCamera;
        private Quaternion _uprightRot;
        private Quaternion _pourRot;

        private bool _hasTriggered = false;

        private readonly float _rotationAngle = 50f;
        private readonly float _rotationSpeed = 200f;

        private void Start()
        {
            _uprightRot = rotationRoot.localRotation;
            _pourRot = Quaternion.Euler(0, 0, _rotationAngle);
        }

        void Update()
        {
            var isHeld = false;
            var mouseClick = Mouse.current.leftButton.isPressed;

            if (TryGetComponent<HoldableObjectBehaviour>(out var h))
                isHeld = h.IsCurrentlyHeld;

            var isMouseDown = isHeld && mouseClick;

            if (isHeld && !_hasTriggered)
            {
                if (audioSource && pickUpSound)
                    audioSource.PlayOneShot(pickUpSound);

                ChoreEvents.TriggerWateringCanPickedUp(wateringCanID);
                _hasTriggered = true;
            }

            rotationRoot.localRotation = Quaternion.RotateTowards(
                rotationRoot.localRotation,
                isMouseDown ? _pourRot : _uprightRot,
                _rotationSpeed * Time.deltaTime
            );
        }
    }
}

