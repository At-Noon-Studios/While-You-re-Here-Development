using UnityEngine;

namespace picking_up_objects
{
    public class HeldObjectController : MonoBehaviour
    {
        [SerializeField] private Transform placeObjectPoint;

        // private Pickable _pickAbleObject; <-- not sure if I stil need this or _heldObject
        private PlayerController _playerController;
    
        private IHeldObject _heldObject;
    
        private void OnDrop()
        {
            if (_heldObject != null)
            {
                _heldObject.Drop();
                _playerController.SetHeldObject(null);
                _heldObject = null;
            }
        }

        private void OnPlace()
        {
            _heldObject?.Place(placeObjectPoint);
        }
    }
}