using UnityEngine;

namespace making_tea
{
    public class WaterTap : MonoBehaviour
    {
        public ParticleSystem waterStream;

        [Header("Audio")]
        public AudioClip waterLoopClip;

        private AudioSource _audio;

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

        private void Awake()
        {
            if (_audio == null)
                _audio = GetComponent<AudioSource>();

            if (_audio == null)
                _audio = gameObject.AddComponent<AudioSource>();

            _audio.loop = true;
            _audio.playOnAwake = false;
        }

        private void Start()
        {
            UpdateWaterState();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (waterStream == null) return;
                
                if (isRunning)
                    waterStream.Play();
                else
                    waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                return;
            }

            UpdateWaterState();
        }

        public void ToggleTap()
        {
            IsRunning = !IsRunning;
        }

        private void UpdateWaterState()
        {
            if (_audio == null)
                return;

            if (isRunning)
            {
                if (waterStream != null && !waterStream.isPlaying)
                    waterStream.Play();

                if (waterLoopClip == null) return;
                
                _audio.clip = waterLoopClip;
                if (!_audio.isPlaying)
                    _audio.Play();
            }
            else
            {
                if (waterStream != null)
                    waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                if (_audio.isPlaying)
                    _audio.Stop();
            }
        }
    }
}
