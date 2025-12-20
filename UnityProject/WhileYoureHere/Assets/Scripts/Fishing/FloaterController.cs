using System.Collections;
using UnityEngine;

namespace Fishing
{
    public class FloaterController : MonoBehaviour
    {

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _rigidbody.isKinematic = true;
            if (other.TryGetComponent<FishingArea>(out var fishingArea))
            {
                StartCoroutine(CatchFish(fishingArea));
            }
            else
            {
                FishingRod.TriggerFishCaught(null);
            }
        }

        private static IEnumerator CatchFish(FishingArea fishingArea)
        {
            yield return new WaitForSeconds(3);
            FishingRod.TriggerFishCaught(fishingArea.GetFish());
        }
    }
}
