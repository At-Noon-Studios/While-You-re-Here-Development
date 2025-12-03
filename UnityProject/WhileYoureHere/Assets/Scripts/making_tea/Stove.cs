using chore;
using UnityEngine;
using System.Collections;

namespace making_tea
{
    public class Stove : MonoBehaviour
    {
        public float requiredFill = 0.2f;
        public float heatTime = 3f;

        [Header("Audio")]
        public AudioSource whistleSound;
        public AudioSource boilingSound;

        [Header("Steam Settings")]
        public float steamStopDelay = 3f;

        private float _heatTimer;
        private Coroutine steamStopRoutine;

        public bool isOn = false;
        private KettleFill currentKettle;  // track the kettle on the stove

        private void OnTriggerStay(Collider other)
        {
            if (!isOn) return;

            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle == null) return;

            currentKettle = kettle;

            if (kettle.fillAmount < requiredFill)
                return;

            var steam = kettle.GetComponentInChildren<ParticleSystem>();

            if (!boilingSound.isPlaying)
                boilingSound.Play();

            _heatTimer += Time.deltaTime;

            if (_heatTimer >= heatTime)
            {
                if (steam != null && !steam.isPlaying)
                    steam.Play();

                if (!whistleSound.isPlaying)
                    whistleSound.Play();

                ChoreEvents.TriggerWaterBoiled();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle == null) return;

            Debug.Log("Stove: kettle left stove");
            StopSteamWithDelay(kettle);
        }

        public void ToggleStove()
        {
            isOn = !isOn;

            if (!isOn)
            {
                Debug.Log("Stove turned OFF");
                if (currentKettle != null)
                    StopSteamWithDelay(currentKettle);
            }
        }

        private void StopSteamWithDelay(KettleFill kettle)
        {
            _heatTimer = 0f;

            if (boilingSound.isPlaying)
                boilingSound.Stop();
            if (whistleSound.isPlaying)
                whistleSound.Stop();

            var steam = kettle.GetComponentInChildren<ParticleSystem>();
            if (steam == null) return;

            if (steamStopRoutine != null)
                StopCoroutine(steamStopRoutine);

            steamStopRoutine = StartCoroutine(StopSteamAfterDelay(steam));
        }

        private IEnumerator StopSteamAfterDelay(ParticleSystem steam)
        {
            yield return new WaitForSeconds(steamStopDelay);

            if (steam != null && steam.isPlaying)
                steam.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
