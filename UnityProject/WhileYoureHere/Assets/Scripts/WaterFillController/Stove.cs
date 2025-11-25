using UnityEngine;

namespace WaterFillController
{
    public class Stove : MonoBehaviour
    {
        public ParticleSystem boilingParticles;
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

            _heatTimer += Time.deltaTime;

            if (_heatTimer >= heatTime && boilingParticles != null && !boilingParticles.isPlaying)
            {
                boilingParticles.Play();
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
        }
    }
}