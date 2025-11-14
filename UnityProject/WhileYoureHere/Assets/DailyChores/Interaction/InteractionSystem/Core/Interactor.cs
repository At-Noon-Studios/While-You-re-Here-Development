using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 5f;
    [SerializeField] Vector3 _raycastOffset = new Vector3(0f, 1f, 0f);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (DoInteractionTest(out IInteractable interactable))
            {
                if (interactable.CanInteract())
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;

        Ray ray = new Ray(transform.position + _raycastOffset, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _castDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, _castDistance))
        {
            interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                return true;
            }

            return false;
        }

        return false;
    }
}
