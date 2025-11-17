using System;
using EventChannels;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraData data;
    
    [Header("Listen to")]
    [SerializeField] private Vector2EventChannel look;

    public event Action<Quaternion> OnRotate;

    private const float CircleDegrees = 360;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        look.OnRaise += OnLookInput;
    }

    private void OnDisable()
    {
        look.OnRaise -= OnLookInput;
    }
    
    private void OnLookInput(Vector2 mouseDelta)
    {
        _yRotation += (mouseDelta.x * data.Sensitivity) % CircleDegrees;
        _xRotation += (-mouseDelta.y * data.Sensitivity) % CircleDegrees;
        _xRotation = Mathf.Clamp(_xRotation, data.MinYAngle, data.MaxYAngle);
        
        var rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        transform.rotation = rotation;
        OnRotate?.Invoke(rotation);
    }
}
