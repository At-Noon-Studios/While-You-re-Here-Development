using chore;
using UnityEngine;

namespace making_tea
{
    public class Stove : MonoBehaviour
    {
        public ParticleSystem boilingParticles;
        public AudioSource whistleSound;
        public AudioSource boilingSound;
        public float requiredFill = 0.2f;
        public float heatTime = 3f;

        [HideInInspector] 
        public bool isOn = false;

        private float _heatTimer = 0f;

        private void OnTriggerStay(Collider other)
        {
            if (!isOn) return;

            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle == null || kettle.fillAmount < requiredFill) return;
            
            if (!boilingSound.isPlaying)
                boilingSound.Play();
            
            _heatTimer += Time.deltaTime;

            if (_heatTimer >= heatTime && boilingParticles != null && !boilingParticles.isPlaying)
            {
                boilingParticles.Play();
                
                if (!whistleSound.isPlaying)
                    whistleSound.Play();
                
                ChoreEvents.TriggerWaterBoiled();

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<KettleFill>() == null) return;
            ResetHeat();
        }

        public void ToggleStove()
        {
            isOn = !isOn;

            if (!isOn)
                ResetHeat();
        }

        private void ResetHeat()
        {
            _heatTimer = 0f;

            if (boilingParticles != null && boilingParticles.isPlaying)
                boilingParticles.Stop();
            
            if (whistleSound.isPlaying)
                whistleSound.Stop();
            
            if (boilingSound.isPlaying)
                boilingSound.Stop();
        }
    }
}