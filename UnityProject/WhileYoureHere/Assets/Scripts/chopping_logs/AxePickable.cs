using Interactable;
using picking_up_objects;
using UnityEngine;

namespace axe
{
    [RequireComponent(typeof(Rigidbody))]
    public class AxePickable : Pickable
    {

        private bool _isHolding;
        
        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with Axe Pickable");
            if (!_isHolding)
            {
                Hold();

                Debug.Log("Axe is now being held");

                // You can add additional logic here specific to the axe
            }
        }
        
    }
}