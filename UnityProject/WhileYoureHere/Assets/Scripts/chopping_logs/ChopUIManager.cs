using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class ChopUIManager : MonoBehaviour
    {
        public static ChopUIManager Instance { get; private set; }

        public static bool IsAxeDown { get; private set; }
        public static bool ShowGuideLine { get; private set; }

        [Header("Sound Settings")]
        [SerializeField] private AudioClip axeImpactSound;

        private AudioSource _audioSource;
        private const float DeltaThresholdUp = 5.0f;
        private const float DeltaThresholdDown = -5.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;

            ResetState();
        }

        public void OnLook(InputValue value)
        {
            if (!Stump.IsCurrentMinigameActive)
            {
                ResetState();
                return;
            }

            var yDelta = value.Get<Vector2>().y;

            ShowGuideLine = true;

            if (yDelta > DeltaThresholdUp)
            {
                IsAxeDown = false;
            }
            else if (yDelta < DeltaThresholdDown)
            {
                IsAxeDown = true;
                PlayAxeImpact();
            }
        }

        private void PlayAxeImpact()
        {
            if (axeImpactSound != null)
                _audioSource.PlayOneShot(axeImpactSound);
        }

        private void ResetState()
        {
            IsAxeDown = false;
            ShowGuideLine = false;
        }
    }
}