using door;
using Interactable;
using Interactable.Holdable;
using ScriptableObjects.Gamestate;
using UnityEngine;

namespace Interactable.Concrete.Key
{
    public class Key : HoldableObjectBehaviour
    {
        [SerializeField] private Keyhole[] keyholes;
        public Keyhole[] Keyholes => keyholes;

        [HideInInspector] public bool detectable = true;

        public float Rotation { get; private set; }
        private Quaternion _baseRotation = Quaternion.identity;

        [Header("Pickup Sound (play once)")]
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private SoGamestateFlag pickupSoundPlayedFlag;

        public override void Interact(IInteractor interactor)
        {
            PlayPickupSoundOnce();
            base.Interact(interactor);
        }

        private void PlayPickupSoundOnce()
        {
            if (pickupClip == null || pickupSoundPlayedFlag == null) return;
            if (pickupSoundPlayedFlag.currentValue) return;

            AudioSource.PlayClipAtPoint(pickupClip, transform.position);
            pickupSoundPlayedFlag.currentValue = true;
        }

        public void SetBaseRotation(Quaternion baseRotation) => _baseRotation = baseRotation;

        public void RotateKey(float degrees)
        {
            Rotation += degrees;
            Vector3 axis = _baseRotation * Vector3.forward;
            transform.rotation = Quaternion.AngleAxis(Rotation, axis) * _baseRotation;
        }

        public void ResetRotation()
        {
            Rotation = 0f;
            transform.rotation = _baseRotation;
        }

        public override bool IsDetectableBy(IInteractor interactor)
            => base.IsDetectableBy(interactor) && detectable;
    }
}