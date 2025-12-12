using System;
using gardening;
using Interactable.Concrete.ObjectHolder;
using JetBrains.Annotations;
using ScriptableObjects.Interactable;
using UnityEngine;
using Object = System.Object;

namespace Interactable.Holdable
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObjectBehaviour : InteractableBehaviour, IHoldableObject
    {
        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        [Header("Holdable Data")]
        [SerializeField] private HoldableObjectData data;

        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;

        private Transform _playerCamera;
        private ObjectHolder _currentHolder;

        public float Weight => data.Weight;
        public bool IsCurrentlyHeld => _holder != null;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
            }
        }

        protected void Start()
        {
            _originalLayer = gameObject.layer;
        }

        private void Update()
        {
            if (interactionCanvas != null &&
                interactionCanvas.gameObject.activeSelf &&
                _playerCamera != null)
            {
                interactionCanvas.transform.LookAt(_playerCamera);
                interactionCanvas.transform.Rotate(0f, 180f, 0f);
            }
        }

        public override void Interact(IInteractor interactor)
        {
            PickUp(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        private void PickUp(IInteractor interactor)
        {
            GetComponent<PickUpSound>().PlayPickUpSound();

            if (_currentHolder != null)
            {
                _currentHolder.ClearHeldObject(this);
                _currentHolder = null;
            }
            
            _holder = interactor;
            interactor.HeldObject?.Drop();
            interactor.SetHeldObject(this);
            AttachTo(interactor);
            EnableCollider(false);
        }

        public void Drop()
        {
            if (_holder == null)
                throw new Exception("Tried to drop an item that wasn't being held");

            _currentHolder?.ClearHeldObject(this);
            _currentHolder = null;
            
            _holder.SetHeldObject(null);
            _holder = null;

            Detach();
            EnableCollider(true);
        }

        public void Place(Vector3 position, Quaternion? rotation = null, ObjectHolder holder = null)
        {
            _currentHolder = holder;
            
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

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            bool canInteract = _holder == null;

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}