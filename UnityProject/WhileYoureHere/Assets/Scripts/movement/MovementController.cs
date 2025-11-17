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
    public bool IsInput { get; private set; }

    [Header("Player SO for Movement")]
    [SerializeField] private PlayerData playerData;

    Animator _animator;
    CharacterController _controller;

    //[Header("Variables for the headbob")]
    // Might move these 4 variables into a player-data Scriptable Object... >.>
    //public float walkBobSpeed = 5.0f;
    //public float walkBobAmount = 0.05f;
    //[SerializeField] private float movementSpeed = 2.2f;

    private float _timer;
    private Camera _mainCamera;
    private readonly float _defaultYPos;

    //[SerializeField] PlayerInput playerInput;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _mainCamera = GetComponentInChildren<Camera>();
    }

    void MovePlayer()
    {
        Vector3 movementFinal = new(_moveX, 0.0f, _moveY);
        _controller.Move(playerData.MovementSpeed * Time.deltaTime * movementFinal);
    }

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _moveX = movement.x;
        _moveY = movement.y;
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
        MovePlayer();

        _animator.SetBool("isWalking", _moveY > 0);
        _animator.SetBool("isWalkingBackwards", _moveY < 0);
        _animator.SetBool("isStrafingLeft", _moveX < 0);
        _animator.SetBool("isStrafingRight", _moveX > 0);
    }
}