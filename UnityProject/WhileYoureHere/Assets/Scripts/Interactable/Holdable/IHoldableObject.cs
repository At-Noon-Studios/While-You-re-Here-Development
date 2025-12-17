using UnityEngine;
using Interactable.Concrete.ObjectHolder;

namespace Interactable.Holdable
{
    public interface IHoldableObject : IInteractable
    {
        float Weight { get; }

        void Drop();

        void Place(Vector3 position, Quaternion? rotation = null, ObjectHolderSingle holder = null);
    }
}