using System;
using System.Collections;
using UnityEngine;

namespace Fishing
{
    public class FloaterController : MonoBehaviour
    {

        private Rigidbody _rigidbody;
        private ParticleSystem _ripple;
        private ParticleSystem _splash;
        private bool _blockTrigger;
        public static event Action<bool> OnFishSplashing;
        public static void TriggerFishSplashing(bool splashing) =>  OnFishSplashing?.Invoke(splashing);

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _ripple = transform.Find("Ripple").gameObject.GetComponent<ParticleSystem>();
            _splash = transform.Find("Splash").gameObject.GetComponent<ParticleSystem>();
            OnFishSplashing += PlaySplashAnimation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_blockTrigger) return;
            _blockTrigger = true;

            _rigidbody.isKinematic = true;
            if (other.TryGetComponent<FishingArea>(out var fishingArea))
            {
                StartCoroutine(CatchFish(fishingArea));
            }
            else
            {
                FishingRod.TriggerFishCaught(null);
                _blockTrigger = false;
            }
        }

        private IEnumerator CatchFish(FishingArea fishingArea)
        {
            yield return new WaitForSeconds(3);
            FishingRod.TriggerFishCaught(fishingArea.GetFish());
            _blockTrigger = false;
            _ripple.Play();
        }

        private void PlaySplashAnimation(bool splashing)
        {
            if (splashing) _splash.Play();
            else _splash.Stop();
        }
    }
}
