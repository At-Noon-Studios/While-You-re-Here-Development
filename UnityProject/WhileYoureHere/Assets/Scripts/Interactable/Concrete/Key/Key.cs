using door;
using Interactable.Holdable;
using UnityEngine;

namespace Interactable.Concrete.Key
{
    public class Key : HoldableObjectBehaviour
    {
        [SerializeField] private Keyhole[] keyholes;
        public Keyhole[] Keyholes  => keyholes;
        
        [HideInInspector] public bool detectable = true;
        
        public float Rotation { get; private set; }

        public void RotateKey(float degrees)
        {
            Rotation += degrees;
            var currentRotation = transform.rotation;
            transform.rotation =  Quaternion.Euler(currentRotation.x, currentRotation.y, Rotation);
        }

        public void ResetRotation() => Rotation = 0;

        public override bool IsDetectableBy(IInteractor interactor) => base.IsDetectableBy(interactor) && detectable;
    }
}