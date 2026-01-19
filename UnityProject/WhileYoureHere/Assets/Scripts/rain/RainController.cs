

namespace rain
{
    using UnityEngine;

    public class RainController : MonoBehaviour
    {
        [Header("Particle Systems for rain")]
        [SerializeField] private ParticleSystem[] rainParticles;

        [SerializeField] public bool isRaining = false;

        public bool IsRaining
        {
            set => isRaining = value;
        }

        private void Start()
        {
            if (isRaining) TriggerStartRain();
            else TriggerStopRain();
        }

        public void TriggerStartRain()
        {
            foreach (var rain in rainParticles)
            {
                if (rain != null) rain.Play();
                isRaining = true;
            }
        }

        public void TriggerStopRain()
        {
            foreach (var rain in rainParticles)
            {
                if (rain != null) rain.Stop();
                isRaining = false;
            }
        }
    }
}
