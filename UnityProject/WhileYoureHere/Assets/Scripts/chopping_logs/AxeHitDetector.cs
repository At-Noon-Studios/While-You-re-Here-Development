using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class AxeHitDetector : MonoBehaviour
    {
        private Vector3 _lastPos;
        public float SwingSpeed { get; private set; }

        [Header("Axe Swing Settings")]
        [SerializeField] private Transform axeModel;
        [SerializeField] private float swingSensitivity = 2f;
        [SerializeField] private Stump stump;

        public float minZRotation = -45f;
        public float maxZRotation = 45f;

        private float _axeZRotation;
        private Quaternion _baseRotation;
        private bool baseRotationSet = false;

        private void Update()
        {
            SwingSpeed = Vector3.Distance(transform.position, _lastPos) / Time.deltaTime;
            _lastPos = transform.position;
        }
        
        public void SetBaseRotation()
        {
            _baseRotation = axeModel.localRotation;
            baseRotationSet = true;
        }
        
        public void OnSwing(InputValue value)
        {
            if (!stump.MinigameActive) return;
            if (!baseRotationSet) return;

            var delta = value.Get<Vector2>();
            
            _axeZRotation += delta.y * swingSensitivity;
            _axeZRotation = Mathf.Clamp(_axeZRotation, minZRotation, maxZRotation);
            
            axeModel.localRotation = _baseRotation * Quaternion.Euler(0f, 0f, _axeZRotation);

            Debug.Log($"Axe rotated: {_axeZRotation}°");
        }

        public void ConsumeHit()
        {
            SwingSpeed = 0f;
        }
    }
}