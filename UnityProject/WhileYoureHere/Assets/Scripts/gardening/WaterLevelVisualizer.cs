using making_tea;
using UnityEngine;

namespace gardening
{
    public class WaterLevelVisualizer : MonoBehaviour
    {
        public KettleFill wateringCan;

        public float emptyY = 0f;
        public float fullY = 1.5f;

        public Collider waterSurfaceCollider;

        private void Start()
        {
            if (wateringCan == null)
                wateringCan = GetComponentInParent<KettleFill>();

            emptyY = transform.localPosition.y;
        }

        private void Update()
        {
            if (!wateringCan) return;
            
            //Debug.Log($"fillAmount: {wateringCan.fillAmount}, maxFill: {wateringCan.maxFill}");
            
            var t = Mathf.Clamp01(wateringCan.fillAmount / wateringCan.maxFill);
            var pos = transform.localPosition;
            pos.y = Mathf.Lerp(emptyY, fullY, t);
            transform.localPosition = pos;
            
            waterSurfaceCollider.isTrigger = !(wateringCan.fillAmount >= wateringCan.maxFill);
        }
    }
}
