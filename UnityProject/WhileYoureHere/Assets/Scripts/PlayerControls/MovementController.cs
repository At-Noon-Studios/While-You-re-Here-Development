using ScriptableObjects.Controls;
using ScriptableObjects.Events;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace PlayerControls
{
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

        CharacterController _controller;
        
        private float _timer;
        private CameraController _cameraController;
        private float _defaultYPos;
        private float _speedModifier = 1f;

        public bool canMove = true;
        
        public void SetMovementModifier(float modifier)
        {
            _speedModifier = modifier;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _cameraController = GetComponentInChildren<CameraController>();
            _defaultYPos = _cameraController.transform.localPosition.y;
        }

        private void OnEnable()
        {
            _cameraController.OnRotate += RotatePlayerBody;
            move.OnRaise += OnMoveInput;
        }

        private void OnDisable()
        {
            _cameraController.OnRotate -= RotatePlayerBody;
            move.OnRaise -= OnMoveInput;
        }

        private void OnMoveInput(Vector2 movementVector)
        {
            _moveX = movementVector.x;
            _moveY = movementVector.y;
        }
    
        
        private void HeadBob()
        {
            if (!IsInput) return;
            _timer += Time.deltaTime * playerData.WalkBobSpeed;
            
            _cameraController.transform.localPosition = new Vector3(
                _cameraController.transform.localPosition.x, 
                _defaultYPos + Mathf.Sin(_timer) * playerData.WalkBobAmount, 
                _cameraController.transform.localPosition.z);
        }

        private void Update()
        {
            if (!canMove) return;

            IsInput = _moveY != 0 || _moveX != 0;
            HeadBob();

            var speed = movementSpeed * _speedModifier;
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
        }

        public void ResumeMovement()
        {
            canMove = true;
        }
    }
}