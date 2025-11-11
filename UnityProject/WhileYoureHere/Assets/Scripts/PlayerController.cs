using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController playerCamera;

    private void OnEnable()
    {
        playerCamera.OnRotate += RotatePlayerBody;
    }

    private void OnDisable()
    {
        playerCamera.OnRotate -= RotatePlayerBody;
    }

    private void RotatePlayerBody(Quaternion rotation)
    {
        var currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, rotation.eulerAngles.y, currentRotation.z);
    }
}
