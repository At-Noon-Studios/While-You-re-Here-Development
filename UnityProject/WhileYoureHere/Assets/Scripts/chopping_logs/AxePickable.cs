using Interactable;
using picking_up_objects;
using UnityEngine;

namespace axe
{
    [RequireComponent(typeof(Rigidbody))]
    public class AxePickable : Pickable
    {

        private bool _isHolding;
        private bool _madeContact;
        
        
        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with Axe Pickable");
            if (!_isHolding)
            {
                _isHolding = true;
                Hold();
                
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!_madeContact)
            {
                _madeContact = true;
                Debug.Log("Axe made contact with log");
                // Additional logic for when the axe makes contact while being held
            }
        }
        
    }
}