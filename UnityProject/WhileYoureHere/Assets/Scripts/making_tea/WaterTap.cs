using UnityEngine;

namespace making_tea
{
    public class WaterTap : MonoBehaviour
    {
        public ParticleSystem waterStream;
    
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
            }
            else
            {
                waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}