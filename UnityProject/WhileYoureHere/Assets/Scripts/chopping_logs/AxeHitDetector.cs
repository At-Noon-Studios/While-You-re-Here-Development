using UnityEngine;

namespace chopping_logs
{
    public class AxeHitDetector : MonoBehaviour
    {
        private Vector3 _lastPos;

        public float SwingSpeed { get; private set; }

        private void Update()
        {
            SwingSpeed = Vector3.Distance(transform.position, _lastPos) / Time.deltaTime;
            _lastPos = transform.position;
        }

        public void ConsumeHit()
        {
            SwingSpeed = 0f;
        }
    }
}