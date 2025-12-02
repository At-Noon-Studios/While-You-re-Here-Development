using System;
using JetBrains.Annotations;
using ScriptableObjects.Interactable;
using UnityEngine;

namespace Interactable.Holdable
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObjectBehaviour : InteractableBehaviour, IHoldableObject
    {
        [SerializeField] private HoldableObjectData data;
        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;

        public float Weight => data.Weight;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        protected void Start()
        {
            _originalLayer = gameObject.layer;
        }
        
        public override void Interact(IInteractor interactor)
        {
            PickUp(interactor);
        }

        private void PickUp(IInteractor interactor)
        {
            _holder = interactor;
            interactor.HeldObject?.Drop();
            interactor.SetHeldObject(this);
            AttachTo(interactor);
            EnableCollider(false);
        }
        
        public void Drop()
        {
            if (_holder == null) throw new Exception("Tried to drop an item that wasn't being held");
            _holder.SetHeldObject(null);
            _holder = null;
            Detach();
            EnableCollider(true);
        }

        public void Place(Vector3 position, Quaternion? rotation = null)
        {
            _holder?.SetHeldObject(null);
            _holder = null;
            _rigidbody.isKinematic = true;
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
            transform.position = position;
            transform.rotation = rotation ?? Quaternion.identity;
            EnableCollider(true);
        }
        
        private void AttachTo(IInteractor interactor)
        {
            _rigidbody.isKinematic = true;
            transform.SetParent(interactor.HoldPoint);
            transform.localRotation = Quaternion.Euler(data.Rotation);
            transform.localPosition = data.Offset;
        }

        private void Detach()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce((transform.parent?.forward ?? Vector3.zero) * data.DroppingForce);
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
        }
    }
}