using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public class KettlePour : MonoBehaviour
    {
        public KettleFill kettle;
        public ParticleSystem pourStream;
        public Transform pivot;

        [Header("Audio")]
        [SerializeField] private AudioClip pourClip;

        [Header("Pour Settings")]
        public float pourAngle = 120f;
        public float rotateSpeed = 8f;
        public float pourSpeed = 0.25f;

        private AudioSource _audio;
        private Quaternion _uprightRot;
        private Quaternion _pourRot;
        private bool _wasPouring;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (pivot == null)
                pivot = transform;

            _uprightRot = pivot.localRotation;
            _pourRot = Quaternion.Euler(
                pivot.localEulerAngles.x,
                pivot.localEulerAngles.y,
                pivot.localEulerAngles.z - pourAngle
            );
        }

        private void Update()
        {
            var isHeld = TryGetComponent<HoldableObjectBehaviour>(out var h) && h.IsCurrentlyHeld;
            var isTableHeld = TryGetComponent<KettleTablePickup>(out var t) && t.IsTableHeld;

            var wantsPour =
                (isHeld || isTableHeld) &&
                Mouse.current != null &&
                Mouse.current.leftButton.isPressed &&
                kettle != null &&
                kettle.fillAmount > 0f;

            if (wantsPour)
            {
                if (!_wasPouring && pourClip != null)
                    _audio.PlayOneShot(pourClip);

                _wasPouring = true;

                pivot.localRotation = Quaternion.Lerp(
                    pivot.localRotation, _pourRot, Time.deltaTime * rotateSpeed);

                if (pourStream && !pourStream.isPlaying)
                    pourStream.Play();

                kettle.fillAmount = Mathf.Max(0f, kettle.fillAmount - pourSpeed * Time.deltaTime);
            }
            else
            {
                _wasPouring = false;

                pivot.localRotation = Quaternion.Lerp(
                    pivot.localRotation, _uprightRot, Time.deltaTime * rotateSpeed);

                if (pourStream && pourStream.isPlaying)
                    pourStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
