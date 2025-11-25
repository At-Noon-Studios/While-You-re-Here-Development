using UnityEngine;

namespace WaterFillController
{
    public class WaterFillZone : MonoBehaviour
    {
        public WaterTap tap;

        private void OnTriggerStay(Collider other)
        {
            var kettle = other.GetComponentInParent<KettleFill>();

            if (kettle == null) return;
            if (tap.isRunning)
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