using UnityEngine;
using player_controls;   // Für MovementController, CameraController, PlayerInteractionController
using Interactable;      // Für IInteractable, IInteractor

public class ChairInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform sitPoint;
    [SerializeField] private Transform lookTarget;

    private bool isSitting = false;

    private PlayerInteractionController pic;
    private MovementController movement;
    private CameraController cameraController;
    private Transform player;
    private Camera playerCam;

    public bool InteractableBy(IInteractor interactor)
    {
        return true;
    }

    public string InteractionText(IInteractor interactor)
    {
        return isSitting ? "[F] Stand up" : "[E] Sit";
    }

    public void EnableCollider(bool enable)
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = enable;
    }

    public void OnHoverEnter(IInteractor interactor)
    {
    }

    public void OnHoverExit(IInteractor interactor)
    {
    }

    // Haupt-Interact-Methode
    public void Interact(IInteractor interactor)
    {
        if (!isSitting)
            Sit(interactor);
        else
            StandUp();
    }
    
    private void Sit(IInteractor interactor)
    {
        var p = interactor as PlayerInteractionController;
        if (p == null)
        {
            Debug.LogWarning("ChairInteractable: Interactor is not a PlayerInteractionController!");
            return;
        }

        player = p.transform;
        movement = p.GetComponent<MovementController>();
        playerCam = p.GetComponentInChildren<Camera>();
        cameraController = p.GetComponentInChildren<CameraController>();

        if (movement != null) movement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        player.position = sitPoint.position;
        player.rotation = sitPoint.rotation;

        if (lookTarget != null && playerCam != null)
        {
            Vector3 dir = lookTarget.position - playerCam.transform.position;
            playerCam.transform.rotation = Quaternion.LookRotation(dir);
        }

        isSitting = true;
        
        pic = interactor as PlayerInteractionController;
        pic.EnableTableMode(true);

    }

    private void StandUp()
    {
        if (movement != null) movement.enabled = true;
        if (cameraController != null) cameraController.enabled = true;

        isSitting = false;
        
        pic.EnableTableMode(false);
        
    }
}
