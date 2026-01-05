using Interactable.Holdable;
using UnityEngine;

namespace Interactable
{
    public interface IInteractor
    {
        public IHoldableObject HeldObject { get; }
        
        public void SetHeldObject(IHoldableObject holdableObject);
        
        public Transform HoldPoint { get; }
    }
}