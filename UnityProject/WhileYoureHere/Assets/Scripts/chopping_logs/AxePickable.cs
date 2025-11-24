using Interactable;
using picking_up_objects;
using UnityEngine;

namespace axe
{
    public class AxePickable : Pickable
    {

        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with Axe Pickable");
            base.Interact();
        }
        
    }
}