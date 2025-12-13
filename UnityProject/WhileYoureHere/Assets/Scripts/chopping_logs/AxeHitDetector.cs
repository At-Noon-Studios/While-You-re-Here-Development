using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class AxeHitDetector : MonoBehaviour
    {
        [Header("Axe Swing Settings")]
        [SerializeField] private float swingSensitivity = 2f;
        [SerializeField] private float minZRotation = -45f;
        [SerializeField] private float maxZRotation = 45f;

        [Header("Transform References")]
        [SerializeField] private Transform axeModel;
        [SerializeField] private Stump stump;
        
        private float _axeZRotation;
        private Vector3 _lastPos;
        
        private Quaternion _baseRotation;
        private bool _isBaseRotationSet;

        private void Update()
        {
            _lastPos = transform.position;
        }
        
        public void SetBaseRotation()
        {
            _baseRotation = axeModel.localRotation;
            _isBaseRotationSet = true;
        }
        
        public void OnSwing(InputValue value)
        {
            if (!stump.IsMinigameActive) return;
            if (!_isBaseRotationSet) return;

            var delta = value.Get<Vector2>();
            
            _axeZRotation += delta.y * swingSensitivity;
            _axeZRotation = Mathf.Clamp(_axeZRotation, minZRotation, maxZRotation);
            
            axeModel.localRotation = _baseRotation * Quaternion.Euler(0f, 0f, _axeZRotation);
        }
    }
}