using System;
using ScriptableObjects.Controls;
using ScriptableObjects.Events;
using UnityEngine;

namespace player_controls
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraData data;
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;

        public event Action<Quaternion> OnRotate;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            SubscribeLook();
        }

        private void OnEnable() => SubscribeLook();
        private void OnDisable() => UnsubscribeLook();

        private void SubscribeLook()
        {
            if (_isLookSubscribed) return;
            look.OnRaise += OnLookInput;
            _isLookSubscribed = true;
        }

        private void UnsubscribeLook()
        {
            if (!_isLookSubscribed) return;
            look.OnRaise -= OnLookInput;
            _isLookSubscribed = false;
        }

        private void OnLookInput(Vector2 mouseDelta)
        {
            _yRotation += (mouseDelta.x * data.Sensitivity) % CircleDegrees;
            _xRotation += (-mouseDelta.y * data.Sensitivity) % CircleDegrees;
            _xRotation = Mathf.Clamp(_xRotation, data.MinYAngle, data.MaxYAngle);

            var rot = Quaternion.Euler(_xRotation, _yRotation, 0);
            transform.rotation = rot;

            OnRotate?.Invoke(rot);
        }

        public void PauseCameraMovement()
        {
            OnDisable();
        }
        
        public void ResumeCameraMovement()
        {
            OnEnable();
        }
    }
}