using UnityEngine;
using Interactable.Concrete.ObjectHolder;

namespace Interactable.Holdable
{
    public interface IHoldableObject : IInteractable
    {
        public float Weight { get; }

        public void Drop();

        public void Place(Vector3 position, Quaternion? rotation = null, ObjectHolderSingle holder = null);
    }
}