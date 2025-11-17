using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("Player SO for Movement")]
    [SerializeField] private PlayerData playerData;

    private float _moveX;
    private float _moveY;
    public bool IsInput { get; private set; }

    [Header("Listen to")]
    [SerializeField] private Vector2EventChannel move;

    [SerializeField] private float movementSpeed = 2.2f;

    Animator _animator;
    CharacterController _controller;

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
        if (IsInput)
        {
            _timer += Time.deltaTime * playerData.WalkBobSpeed;
            _mainCamera.transform.localPosition = new Vector3(_mainCamera.transform.localPosition.x, _defaultYPos + Mathf.Sin(_timer) * playerData.WalkBobAmount, _mainCamera.transform.localPosition.z);
        }
    }

    void Update()
    {
        IsInput = _moveY != 0 || _moveX != 0;

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