using chore;
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

        private Rigidbody _rb;
        private Quaternion _uprightRot;
        private Quaternion _pourRot;
        
        private bool _hasTriggered = false;
        private bool _isFilled = false;

        private readonly float _rotationAngle = 60f;
        private readonly float _rotationSpeed = 200f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            _uprightRot = rotationRoot.localRotation;
            _pourRot = Quaternion.Euler(0, 0, _rotationAngle);
        }

        void Update()
        {
            var isHolding = !_rb.useGravity;
            var mouseClick = Mouse.current.leftButton.isPressed;

            var isMouseDown = isHolding && mouseClick;

            if (isHolding && !_hasTriggered)
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

