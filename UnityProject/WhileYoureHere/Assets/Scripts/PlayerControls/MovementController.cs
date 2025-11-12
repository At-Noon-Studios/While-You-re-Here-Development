using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    private float _moveX;
    private float _moveY;
    private readonly bool _isInput;
    
    [Header("Listen to")]
    [SerializeField] private Vector2EventChannel move;

    [SerializeField] private float movementSpeed = 2.2f;

    Animator _animator;
    CharacterController _controller;

    [Header("Variables for the headbob")]
    public float walkBobSpeed = 5.0f;
    public float walkBobAmount = 0.05f;
    private float _timer;

    private Camera _mainCamera;
    private CameraController _cameraController;
    private float _defaultYPos;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _mainCamera = GetComponentInChildren<Camera>();
        _cameraController = _mainCamera.gameObject.GetComponent<CameraController>();
        _defaultYPos = _mainCamera.transform.localPosition.y;
    }

    void OnEnable()
    {
        _cameraController.OnRotate += RotatePlayerBody;
        move.OnRaise += OnMoveInput;
    }

    void OnDisable()
    {
        _cameraController.OnRotate -= RotatePlayerBody;
        move.OnRaise -= OnMoveInput;
    }
    
    private void OnMoveInput(Vector2 movementVector)
    {
        _moveX = movementVector.x;
        _moveY = movementVector.y;
    }

    void HeadBob()
    {
        bool _isInput = _moveY != 0 || _moveX != 0;
        if(_isInput)
        {
            _timer += Time.deltaTime * walkBobSpeed;
            _mainCamera.transform.localPosition = new Vector3(_mainCamera.transform.localPosition.x, _defaultYPos + Mathf.Sin(_timer) * walkBobAmount, _mainCamera.transform.localPosition.z);
        }
    }

    void Update()
    {
        HeadBob();

        _animator.SetBool("isWalking", _moveY > 0);
        _animator.SetBool("isWalkingBackwards", _moveY < 0);
        _animator.SetBool("isStrafingLeft", _moveX < 0);
        _animator.SetBool("isStrafingRight", _moveX > 0);

        Vector3 movementFinal = transform.right * _moveX + transform.forward * _moveY;
        _controller.Move(movementSpeed * Time.deltaTime * movementFinal);
    }  
    
    private void RotatePlayerBody(Quaternion rotation)
    {
        var currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, rotation.eulerAngles.y, currentRotation.z);
    }
}