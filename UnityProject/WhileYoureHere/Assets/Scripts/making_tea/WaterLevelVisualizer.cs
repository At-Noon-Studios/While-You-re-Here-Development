using UnityEngine;

namespace making_tea
{
    public class WaterLevelVisualizer : MonoBehaviour
    {
        [Header("Water Level References")]
        public KettleFill kettle;
        public Collider waterSurfaceCollider;

        [Header("Water Level Positions")]
        public float emptyYPos;
        public float fullYPos;

        private void Start()
        {
            if (kettle == null)
                kettle = GetComponentInParent<KettleFill>();

            emptyYPos = transform.localPosition.y;
        }

        private void Update()
        {
            if (!kettle) return;
            
            var t = Mathf.Clamp01(kettle.fillAmount / kettle.maxFill);
            var pos = transform.localPosition;
            pos.y = Mathf.Lerp(emptyYPos, fullYPos, t);
            transform.localPosition = pos;
            
            waterSurfaceCollider.isTrigger = !(kettle.fillAmount >= kettle.maxFill);
        }
    }
}