using picking_up_objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Settings")] 
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float strafeSpeed = 5f;

    [Header("Mouse Look Settings")] 
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraTransform;
    
    private float _movementX;
    private float _movementY;
    private float _speedModifier;
    private float _mouseLookRotation;
    
    private IWeighted _heldObject;
    
    void Awake()
    {
        _speedModifier = moveSpeed;
    }
    
    void Update()
    {
        MovePlayer();
        MoveSideways();
        MouseLook();
    }

    void OnMove(InputValue movement)
    {
        var movementVector = movement.Get<Vector2>();
        _movementX = movementVector.x;
        _movementY = movementVector.y;
    }
    
    public void SetHeldObject(IWeighted heldObject)
    {
        _heldObject = heldObject;
    }
    
    private void MoveSideways()
    {
        transform.Translate(_movementX * strafeSpeed * Time.deltaTime, 0f, 0f, Space.World);
    }

    private void MovePlayer()
    {
        _speedModifier = moveSpeed;

        if (_heldObject != null)
        {
            _speedModifier = moveSpeed / (_heldObject.Weight + 1f);
            _speedModifier = Mathf.Max(_speedModifier, 1f);
        }

        transform.Translate(0f, 0f, _movementY * _speedModifier * Time.deltaTime);
    }
    
    private void MouseLook()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;
        
        _mouseLookRotation -= mouseY;
        _mouseLookRotation = Mathf.Clamp(_mouseLookRotation, -90f, 90f);
        
        cameraTransform.localRotation = Quaternion.Euler(_mouseLookRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    


}