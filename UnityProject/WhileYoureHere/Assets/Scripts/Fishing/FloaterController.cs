using System.Collections;
using UnityEngine;

namespace Fishing
{
    public class FloaterController : MonoBehaviour
    {

        private Rigidbody _rigidbody;
        private ParticleSystem _particleSystem;
        private bool _blockTrigger;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _particleSystem = GetComponent<ParticleSystem>();
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
            _particleSystem.Play();
        }
    }
}
