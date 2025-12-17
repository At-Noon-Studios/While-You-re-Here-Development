using UnityEngine;

namespace making_tea
{
    public class MugFillZone : MonoBehaviour
    {
        [Header("Fill Settings")]
        public KettleFill cupFill;
        public float stopFillDelay = 0.1f;

        private float _lastHitTime;

        private void Awake()
        {
            if (cupFill == null)
                cupFill = GetComponentInParent<KettleFill>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!other.CompareTag("KettleWater"))
                return;

            if (cupFill == null) return;

            _lastHitTime = Time.time;
            cupFill.StartFilling();
        }

        private void Update()
        {
            if (cupFill == null) return;

            if (cupFill.isFilling && Time.time - _lastHitTime > stopFillDelay)
                cupFill.StopFilling();
        }
    }
}