using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace chopping_logs
{
    public class ChopUIManager : MonoBehaviour
    {
        public static ChopUIManager Instance { get; private set; }

        [Header("Sound Settings")] 
        [SerializeField] private AudioClip axeImpactSound;

        [Header("UI References")] 
        [SerializeField] private Image mouseUpImage;
        [SerializeField] private Image mouseDownImage;
        [SerializeField] private Image guideLineImage;

        private AudioSource _audioSource;
        private bool _isAxeDown;
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

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f;
            }

            HideAllUI();
        }

        public void OnLook(InputValue value)
        {
            if (!Stump.IsCurrentMinigameActive) return;

            var delta = value.Get<Vector2>();
            var yDelta = delta.y;

            if (guideLineImage != null && !guideLineImage.enabled)
            {
                guideLineImage.enabled = true;
            }

            var previousAxeDown = _isAxeDown;

            if (yDelta > DeltaThresholdUp)
            {
                _isAxeDown = false;
                if (mouseUpImage != null) mouseUpImage.enabled = false;
                if (mouseDownImage != null) mouseDownImage.enabled = true;
            }
            else if (yDelta < DeltaThresholdDown)
            {
                _isAxeDown = true;
                if (mouseUpImage != null) mouseUpImage.enabled = true;
                if (mouseDownImage != null) mouseDownImage.enabled = false;
            }

            if (_isAxeDown && !previousAxeDown)
            {
                PlayAxeImpact();
            }
        }

        private void PlayAxeImpact()
        {
            if (axeImpactSound == null || _audioSource == null) return;
            _audioSource.PlayOneShot(axeImpactSound);
        }

        public void ShowUI()
        {
            if (mouseUpImage != null) mouseUpImage.enabled = true;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = true;
        }

        public void HideAllUI()
        {
            if (mouseUpImage != null) mouseUpImage.enabled = false;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = false;
        }
    }
}