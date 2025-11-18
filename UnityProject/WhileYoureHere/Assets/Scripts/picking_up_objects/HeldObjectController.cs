using UnityEngine;
using UnityEngine.InputSystem;

namespace picking_up_objects
{
    public class HeldObjectController : MonoBehaviour
    {
        [SerializeField] private Transform placeObjectPoint;
        
        private PlayerController _playerController;
        private IHeldObject _heldObject;
        
        private Pickable _pickable;


        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _heldObject = null;
                _playerController.SetHeldObject(null);
            }
        }

        private void OnDrop()
        {
            if (_heldObject != null)
            {
                _pickable.Drop();
                _playerController.SetHeldObject(null);
                _heldObject = null;
            }
        }

        private void OnPlace()
        {
            _pickable?.Place(placeObjectPoint);
        }
    }
}