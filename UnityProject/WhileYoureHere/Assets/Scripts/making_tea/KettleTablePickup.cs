using UnityEngine;
using Interactable;

public class KettleTablePickup : MonoBehaviour, IInteractable
{
    private bool lifted = false;
    private PlayerInteractionController pic;

    public float liftHeight = 1.0f;

    public bool IsTableHeld => lifted;

    public string InteractionText(IInteractor i)
        => lifted ? "[E] Drop" : "[E] Pick up";

    public bool InteractableBy(IInteractor i)
    {
        var p = i as PlayerInteractionController;
        return p != null && p.TableMode;
    }

    public void EnableCollider(bool s)
    {
        var c = GetComponent<Collider>();
        if (c) c.enabled = s;
    }

    public void OnHoverEnter(IInteractor i) {}
    public void OnHoverExit(IInteractor i) {}

    public void Interact(IInteractor i)
    {
        var p = i as PlayerInteractionController;
        if (p == null) return;

        pic = p;
        lifted = !lifted;

        if (!lifted)
        {
            transform.position = new Vector3(
                transform.position.x,
                0.75f,
                transform.position.z
            );
        }
    }

    private void Update()
    {
        if (!lifted || pic == null || !pic.TableMode) return;

        var cam = pic.PlayerCamera;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            Vector3 pos = hit.point;
            pos.y = liftHeight;
            transform.position = pos;
        }

        // gießen über KettlePour (TableMode-Hold)
        if (UnityEngine.Input.GetMouseButton(0))
        {
            var pour = GetComponent<making_tea.KettlePour>();
            if (pour != null)
                pour.enabled = true;
        }
    }
}