using UnityEngine;
using UnityEngine.InputSystem;

namespace picking_up_objects
{
    public class HeldObjectController : MonoBehaviour
    {
        // [SerializeField] private Transform placeObjectPoint;
       // public IHeldObject HeldObject => _heldObject;
        private IHeldObject _heldObject;
        
        public void SetHeldObject(IHeldObject heldObject)
        {
            _heldObject = heldObject;
        }
        
        private void OnDrop()
        {
            if (_heldObject != null)
            {
                _heldObject.Drop(); 
                _heldObject = null;
            }
        }

        private void OnPlace()
        {
            if (_heldObject != null)
            {
                _heldObject.Place();
                _heldObject = null;
            }
        }
    }
}