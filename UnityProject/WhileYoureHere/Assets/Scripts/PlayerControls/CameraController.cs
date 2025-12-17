using System;
using ScriptableObjects.Controls;
using ScriptableObjects.Events;
using UnityEngine;

namespace PlayerControls
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraData data;
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;
        
        private const float CircleDegrees = 360;

        public event Action<Quaternion> OnRotate;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable() => SubscribeLook();
        private void OnDisable() => UnsubscribeLook();

        private void SubscribeLook()
        {
            look.OnRaise += OnLookInput;
        }

        private void UnsubscribeLook()
        {
            look.OnRaise -= OnLookInput;
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
        
        public void SyncRotation(Quaternion worldRotation)
        {
            Vector3 euler = worldRotation.eulerAngles;

            _xRotation = NormalizeAngle(euler.x);
            _yRotation = NormalizeAngle(euler.y);

            transform.rotation = worldRotation;
        }
        
        private float NormalizeAngle(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }
        
        public void SetMinigameRotation(Quaternion rotation)
        {
            transform.rotation = rotation;

            Vector3 euler = rotation.eulerAngles;
            _xRotation = NormalizeAngle(euler.x);
            _yRotation = NormalizeAngle(euler.y);

            PauseCameraMovement();
        }
    }
}