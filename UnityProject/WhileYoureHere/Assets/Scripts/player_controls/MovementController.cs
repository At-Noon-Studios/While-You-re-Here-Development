using EventChannels;
using picking_up_objects;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace player_controls
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        private static readonly int IsWalkingBackwards = Animator.StringToHash("isWalkingBackwards");
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsStrafingLeft = Animator.StringToHash("isStrafingLeft");
        private static readonly int IsStrafingRight = Animator.StringToHash("isStrafingRight");

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

        private IHeldObject _heldObject;
        private float _timer;
        private Camera _mainCamera;
        private CameraController _cameraController;
        private float _defaultYPos;
        private float _speedModifier = 1f;

        public bool canMove = true;
        
        public void SetMovementModifier(float modifier)
        {
            _speedModifier = modifier;
        }

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
            
                _mainCamera.transform.localPosition = new Vector3(
                    _mainCamera.transform.localPosition.x, 
                    _defaultYPos + Mathf.Sin(_timer) * playerData.WalkBobAmount, 
                    _mainCamera.transform.localPosition.z);
            }
        }

        void Update()
        {
            if (!canMove) return;

            IsInput = _moveY != 0 || _moveX != 0;
            HeadBob();

            _animator.SetBool(IsWalking, _moveY > 0);
            _animator.SetBool(IsWalkingBackwards, _moveY < 0);
            _animator.SetBool(IsStrafingLeft, _moveX < 0);
            _animator.SetBool(IsStrafingRight, _moveX > 0);

            float speed = movementSpeed * _speedModifier;
            var movementFinal = transform.right * _moveX + transform.forward * _moveY + Physics.gravity;
            _controller.Move(speed * Time.deltaTime * movementFinal);
        }

        private void RotatePlayerBody(Quaternion rotation)
        {
            var currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotation.x, rotation.eulerAngles.y, currentRotation.z);
        }

        public void PauseMovement()
        {
            canMove = false;
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsWalkingBackwards, false);
            _animator.SetBool(IsStrafingLeft, false);
            _animator.SetBool(IsStrafingRight, false);
        }

        public void ResumeMovement()
        {
            canMove = true;
        }
    }
}