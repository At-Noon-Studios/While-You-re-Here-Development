using gardening;
using Interactable.Concrete.ObjectHolder;
using UnityEngine;

namespace Interactable.Holdable
{
    public interface IHoldableObject : IInteractable
    {
        public float Weight { get; }

        public void Drop();
        
        public void Place(Vector3 position, Quaternion? rotation = null, WateringCanHolder holder = null);
    }
}