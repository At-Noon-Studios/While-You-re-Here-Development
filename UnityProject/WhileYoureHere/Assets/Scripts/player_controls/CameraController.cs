using System;
using EventChannels;
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

        private bool _lookSubscribed;
        public bool canLook = true;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            SubscribeLook();
        }

        private void OnEnable()     => SubscribeLook();
        private void OnDisable()    => UnsubscribeLook();

        private void SubscribeLook()
        {
            if (_lookSubscribed) return;
            look.OnRaise += OnLookInput;
            _lookSubscribed = true;
        }

        private void UnsubscribeLook()
        {
            if (!_lookSubscribed) return;
            look.OnRaise -= OnLookInput;
            _lookSubscribed = false;
        }

        private void OnLookInput(Vector2 mouseDelta)
        {
            if (!canLook) return;

            _yRotation += mouseDelta.x * data.Sensitivity;
            _xRotation += -mouseDelta.y * data.Sensitivity;
            _xRotation = Mathf.Clamp(_xRotation, data.MinYAngle, data.MaxYAngle);

            var rot = Quaternion.Euler(_xRotation, _yRotation, 0);
            transform.rotation = rot;

            OnRotate?.Invoke(rot);
        }

        public void PauseCameraMovement()
        {
            canLook = false;
            UnsubscribeLook();
        }
        
        public void ResumeCameraMovement()
        {
            canLook = true;
            SubscribeLook();
        }
    }
}