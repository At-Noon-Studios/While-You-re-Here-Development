using UnityEngine;

namespace chopping_logs
{
    public class AxeHitDetector : MonoBehaviour
    {
        public float minSwingSpeed = 1.2f;
        private Vector3 _last;

        public float SwingSpeed { get; private set; }

        private void Update()
        {
            SwingSpeed = Vector3.Distance(transform.position, _last) / Time.deltaTime;
            _last = transform.position;
        }
    }
}