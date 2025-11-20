using axe;
using PlayerControls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace picking_up_objects
{
    public class HeldObjectController : MonoBehaviour
    {
        private IHeldObject _heldObject;
        private MovementController _movementController;
        private bool _hasAxe;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
        }

        public void SetHeldObject(IHeldObject heldObject)
        {
            _heldObject = heldObject;
            _hasAxe = heldObject is AxePickable;
            UpdateMovementSpeed();
        }
        
        public void ClearHeldObject()
        {
            _heldObject = null;
            _movementController.SetMovementModifier(1f);
        }
        
        private void UpdateMovementSpeed()
        {
            if (_heldObject == null)
            {
                _movementController.SetMovementModifier(1f);
                return;
            }

            float weight = Mathf.Clamp01(_heldObject.Weight / 100f);
            float modifier = Mathf.Max(1f - weight, 0.4f);

            _movementController.SetMovementModifier(modifier);
        }
        
        private void OnDrop()
        {
            if (_heldObject != null)
            {
                _heldObject.Drop(); 
                _heldObject = null;
                
                ClearHeldObject();
            }
        }

        private void OnPlace()
        {
            if (_heldObject != null)
            {
                _heldObject.Place();
                _heldObject = null;
                
                ClearHeldObject();
            }
        }
    }
}