using UnityEngine;

namespace making_tea
{
    public class WaterLevelVisualizer : MonoBehaviour
    {
        public KettleFill kettle;

        public float emptyY;
        public float fullY;

        public Collider waterSurfaceCollider;

        private void Start()
        {
            if (kettle == null)
                kettle = GetComponentInParent<KettleFill>();

            emptyY = transform.localPosition.y;
        }

        private void Update()
        {
            if (!kettle) return;
            
            var t = Mathf.Clamp01(kettle.fillAmount / kettle.maxFill);
            var pos = transform.localPosition;
            pos.y = Mathf.Lerp(emptyY, fullY, t);
            transform.localPosition = pos;
            
            waterSurfaceCollider.isTrigger = !(kettle.fillAmount >= kettle.maxFill);
        }
    }
}