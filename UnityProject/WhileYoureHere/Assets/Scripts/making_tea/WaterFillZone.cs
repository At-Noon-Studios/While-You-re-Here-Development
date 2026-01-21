using entity;
using UnityEngine;

namespace making_tea
{
    public class WaterFillZone : MonoBehaviour
    {
        public WaterTap tap;

        private void OnTriggerStay(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle == null) return;

            if (!kettle.CanFill())
            {
                kettle.StopFilling();
                return;
            }

            if (tap.IsRunning)
                kettle.StartFilling();
            else
                kettle.StopFilling();
        }

        private void OnTriggerExit(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();
            if (kettle != null)
                kettle.StopFilling();
        }
    }
}