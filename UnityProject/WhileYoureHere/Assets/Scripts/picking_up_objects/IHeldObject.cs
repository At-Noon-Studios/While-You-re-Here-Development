using UnityEngine;

namespace picking_up_objects
{
    public interface IHeldObject
    {
        public void Drop();

        public void Place();
        
        float Weight { get; }
    }
}