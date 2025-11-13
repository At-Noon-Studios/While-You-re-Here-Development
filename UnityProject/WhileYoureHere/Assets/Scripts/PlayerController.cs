using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController playerCamera;
    [Header("Listen to")]
    [SerializeField] private Vector2EventChannel move;
    
    private CharacterController _characterController;
    private Vector3 _currentMovementVector;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _characterController.Move(_currentMovementVector * Time.deltaTime);
    }

    private void OnEnable()
    {
        playerCamera.OnRotate += RotatePlayerBody;
        move.OnRaise += OnMoveInput;
    }

    private void OnDisable()
    {
        playerCamera.OnRotate -= RotatePlayerBody;
        move.OnRaise -= OnMoveInput;
    }

    private void RotatePlayerBody(Quaternion rotation)
    {
        var currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, rotation.eulerAngles.y, currentRotation.z);
    }

    private void OnMoveInput(Vector2 input)
    {
        _currentMovementVector = new Vector3(input.x, 0, input.y);
    }
}
