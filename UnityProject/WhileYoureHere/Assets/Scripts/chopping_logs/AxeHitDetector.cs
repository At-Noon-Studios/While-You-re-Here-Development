using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class AxeHitDetector : MonoBehaviour
    {
        [Header("Axe Swing Settings")]
        [SerializeField] private Transform axeModel;
        [SerializeField] private float swingSensitivity = 2f;
        [SerializeField] private Stump stump;

        public float minZRotation = -45f;
        public float maxZRotation = 45f;

        private float _axeZRotation;
        private Vector3 _lastPos;
        
        private Quaternion _baseRotation;
        private bool _baseRotationSet;

        private void Update()
        {
            _lastPos = transform.position;
        }
        
        public void SetBaseRotation()
        {
            _baseRotation = axeModel.localRotation;
            _baseRotationSet = true;
        }
        
        public void OnSwing(InputValue value)
        {
            if (!stump.MinigameActive) return;
            if (!_baseRotationSet) return;

            var delta = value.Get<Vector2>();
            
            _axeZRotation += delta.y * swingSensitivity;
            _axeZRotation = Mathf.Clamp(_axeZRotation, minZRotation, maxZRotation);
            
            axeModel.localRotation = _baseRotation * Quaternion.Euler(0f, 0f, _axeZRotation);

            Debug.Log($"Axe rotated: {_axeZRotation}°");
        }
    }
}