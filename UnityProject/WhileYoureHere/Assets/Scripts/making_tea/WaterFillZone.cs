using UnityEngine;

namespace making_tea
{
    public class WaterFillZone : MonoBehaviour
    {
        [Header("Water Tap Reference")]
        public WaterTap tap;

        private void OnTriggerStay(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();

            if (kettle == null) return;
            if (tap.IsRunning)
                kettle.StartFilling();
            else
                kettle.StopFilling();
        }

        private void OnTriggerExit(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle != null) kettle.StopFilling();
        }
    }
}