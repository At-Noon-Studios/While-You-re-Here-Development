using UnityEngine;

namespace making_tea
{
    public class WaterTap : MonoBehaviour
    {
        public ParticleSystem waterStream;

        [Header("Audio")]
        public AudioSource tapSound;

        [SerializeField]
        private bool isRunning;

        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
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
            IsRunning = !IsRunning;
        }

        private void UpdateWaterState()
        {
            if (waterStream == null) return;

            if (isRunning)
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