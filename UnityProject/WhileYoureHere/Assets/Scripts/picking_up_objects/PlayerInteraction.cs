using UnityEngine;
using UnityEngine.InputSystem;

namespace picking_up_objects
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactRange = 5f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private Transform placeObjectPoint;
        private Pickable _pickAbleObject;
        
        void OnHold(InputValue input)
        {
            TryPickup();
        }
        
        void OnDrop(InputValue input)
        {
            _pickAbleObject.DropObject();
        }
        
        void OnPlace(InputValue input)
        {
            _pickAbleObject.PlaceObject(placeObjectPoint);
        }

        void Update()
        {
            if (_pickAbleObject && Mouse.current.rightButton.wasPressedThisFrame)
            {
                _pickAbleObject = null;
            }
        }

        private void TryPickup()
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                Pickable pickup = hit.collider.GetComponent<Pickable>();
                pickup.HoldObject(holdPoint);
                _pickAbleObject = pickup;
            }
        }
        
    }
}