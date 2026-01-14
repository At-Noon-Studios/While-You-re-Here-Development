using Interactable;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public class KettlePour : MonoBehaviour
    {
        [Header("Kettle References")]
        public KettleFill kettle;
        public ParticleSystem pourStream;
        public Transform pivot;

        [Header("Audio")]
        [SerializeField] private AudioClip pourClip;

        [Header("Pour Settings")]
        public float pourAngle = 120f;
        public float rotateSpeed = 8f;
        public float pourSpeed = 0.25f;

        [Header("Pour Target Proximity")]
        [SerializeField] private LayerMask pourTargetMask = ~0;
        [SerializeField] private float pourTargetRadius = 0.25f;
        [SerializeField] private Vector3 pourTargetLocalOffset = new Vector3(-0.15f, 0.05f, 0.10f);

        private readonly Collider[] _pourHits = new Collider[16];

        private AudioSource _audio;
        private PlayerInput _playerInput;
        private PlayerInteractionController _player;

        private Quaternion _uprightRot;
        private Quaternion _pourRot;

        private bool _isPourPressed;
        private bool _wasPouring;

        public bool IsNearPourTarget()
        {
            if (pourTargetRadius <= 0f) return false;

            Vector3 checkPos = transform.TransformPoint(pourTargetLocalOffset);

            int count = Physics.OverlapSphereNonAlloc(
                checkPos,
                pourTargetRadius,
                _pourHits,
                pourTargetMask,
                QueryTriggerInteraction.Collide
            );

            for (int i = 0; i < count; i++)
            {
                var c = _pourHits[i];
                if (c == null) continue;

                if (c.transform == transform || c.transform.IsChildOf(transform))
                    continue;

                return true;
            }

            return false;
        }

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            _playerInput = player.GetComponent<PlayerInput>();
            _player = player.GetComponent<PlayerInteractionController>();

            if (_playerInput == null) return;
            _playerInput.actions["Pour"].performed += ctx => _isPourPressed = true;
            _playerInput.actions["Pour"].canceled += ctx => _isPourPressed = false;
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
            var isTableMode = _player != null && _player.IsTableMode;

            var isHeld = TryGetComponent<HoldableObjectBehaviour>(out var h) && h.IsCurrentlyHeld;
            var isTableHeld = TryGetComponent<KettleTablePickup>(out var t) && t.IsTableHeld;

            var canPour =
                isTableMode &&
                (isHeld || isTableHeld) &&
                kettle != null &&
                kettle.fillAmount > 0f &&
                _isPourPressed &&
                IsNearPourTarget();

            if (canPour)
            {
                if (!_wasPouring && pourClip != null)
                    _audio.PlayOneShot(pourClip);

                _wasPouring = true;

                pivot.localRotation = Quaternion.Lerp(pivot.localRotation, _pourRot, Time.deltaTime * rotateSpeed);

                if (pourStream && !pourStream.isPlaying)
                    pourStream.Play();

                kettle.fillAmount = Mathf.Max(0f, kettle.fillAmount - pourSpeed * Time.deltaTime);
            }
            else
            {
                _wasPouring = false;

                pivot.localRotation = Quaternion.Lerp(pivot.localRotation, _uprightRot, Time.deltaTime * rotateSpeed);

                if (pourStream && pourStream.isPlaying)
                    pourStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
