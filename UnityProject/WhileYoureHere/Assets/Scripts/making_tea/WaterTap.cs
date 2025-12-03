using UnityEngine;

namespace making_tea
{
    public class WaterTap : MonoBehaviour
    {
        public ParticleSystem waterStream;

        [Header("Audio")]
        public AudioSource tapSound;

        [SerializeField]
        private bool _isRunning = false;

        public bool isRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                UpdateWaterState();
            }
        }

        private void Start()
        {
            UpdateWaterState();
        }

        private void OnValidate()
        {
            UpdateWaterState();
        }

        public void ToggleTap()
        {
            isRunning = !isRunning;
        }

        private void UpdateWaterState()
        {
            if (waterStream == null) return;

            if (_isRunning)
            {
                waterStream.Play();

                if (tapSound && !tapSound.isPlaying)
                    tapSound.Play();
            }
            else
            {
                waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                if (tapSound && tapSound.isPlaying)
                    tapSound.Stop();
            }
        }
    }
}