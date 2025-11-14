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
    public bool _isInput;

    [SerializeField] private float movementSpeed = 2.2f;

    Animator _animator;
    CharacterController _controller;

    [Header("Variables for the headbob")]
    public float walkBobSpeed = 5.0f;
    public float walkBobAmount = 0.05f;
    private float _timer;

    private Camera _mainCamera;
    private readonly float _defaultYPos;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _mainCamera = GetComponentInChildren<Camera>();
    }

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _moveX = movement.x;
        _moveY = movement.y;
    }

    void HeadBob()
    {
        bool _isInput = _moveY != 0 || _moveX != 0;
        if (_isInput)
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

        Vector3 movementFinal = new(_moveX, 0.0f, _moveY);
        _controller.Move(movementSpeed * Time.deltaTime * movementFinal);
    }
}