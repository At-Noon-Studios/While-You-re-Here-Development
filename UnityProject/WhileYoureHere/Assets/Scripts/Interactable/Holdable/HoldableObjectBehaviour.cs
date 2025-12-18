using System;
using System.Linq;
using chopping_logs;
using Interactable.Concrete.ObjectHolder;
using JetBrains.Annotations;
using ScriptableObjects.Interactable;
using UnityEngine;

namespace Interactable.Holdable
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObjectBehaviour : InteractableBehaviour, IHoldableObject
    {
        [Header("Holdable Data")]
        [SerializeField] private HoldableObjectData data;

        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;
        [CanBeNull] private GameObject _heldVersion;

        private Transform _playerCamera;
        private ObjectHolderSingle _currentHolder;
        private AnimationInteraction _animationInteraction;
                
        public bool IsPlaced { get; private set; }
        private bool _isLocked;

        private const int HoldLayer = 3;

        public bool IsCurrentlyHeld => _holder != null;
        public float Weight => data.Weight;
        
        private GameObject player;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            Renderers = GetComponentsInChildren<Renderer>();

            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
                
                _animationInteraction = player.GetComponentInChildren<AnimationInteraction>();
            }
        }

        protected void Start()
        {
            _originalLayer = gameObject.layer;
            InitializeHeldVersion();
        }

        public override void Interact(IInteractor interactor)
        {
            if (_isLocked)
                return;
            
            var chopTarget = GetComponentInChildren<LogChopTarget>();
            if (chopTarget != null && chopTarget.IsOnStump)
                return;
            
            if (interactor is PlayerInteractionController pic &&
                (pic.IsTableMode || pic.HeldObject != null))
                return;

            PickUp(interactor);
        }
        
        private void PickUp(IInteractor interactor)
        {
            if (_heldVersion) SetHeldVisual(true, _heldVersion);
            if (TryGetComponent<PickUpSound>(out var sound)) sound.PlayPickUpSound();
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
            IsPlaced = false;
            _animationInteraction.CheckInteraction(gameObject, AnimationInteractionType.Grab);
        }
        
        public void PickUpByInteractor(IInteractor interactor)
        {
            var chopTarget = GetComponentInChildren<LogChopTarget>();
            if (chopTarget != null)
            {
                chopTarget.NotifyPickedUp();
            }

            PickUp(interactor);
        }

        
        private void SetHeldVisual(bool state, GameObject heldVisual) {
            heldVisual.SetActive(state);
            foreach (var r in Renderers)
            {
                r.enabled = !state;
            }
        }
        
        public virtual void Drop()
        {
            if (_holder == null) throw new Exception("Tried to drop an item that wasn't being held");
            if (_heldVersion) SetHeldVisual(false, _heldVersion);
            _holder.SetHeldObject(null);
            _holder = null;
            Detach();
            EnableCollider(true);
            IsPlaced = false;
            _animationInteraction.CheckInteraction(gameObject, AnimationInteractionType.Drop);
        }

        public void Place(Vector3 position, Quaternion? rotation = null, ObjectHolderSingle holder = null)
        {
            if (_heldVersion) SetHeldVisual(false, _heldVersion);
            _currentHolder = holder;
            _holder?.SetHeldObject(null);
            _holder = null; 
            _rigidbody.isKinematic = true;
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
            transform.position = position;
            transform.rotation = rotation ?? Quaternion.identity;
            EnableCollider(true);
            IsPlaced = true;
            _animationInteraction.CheckInteraction(gameObject, AnimationInteractionType.Drop);
        }

        private void AttachTo(IInteractor interactor)
        {
            _rigidbody.isKinematic = true;
            transform.SetParent(interactor.HoldPoint);
            transform.localRotation = Quaternion.Euler(data.HoldingRotation);
            transform.localPosition = data.HoldingOffset;
            gameObject.layer = HoldLayer;
        }
        
        public void ResetPose()
        {
            if (_holder == null) return;

            transform.SetParent(_holder.HoldPoint);
            transform.localPosition = data.HoldingOffset;
            transform.localRotation = Quaternion.Euler(data.HoldingRotation);

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
        }


        private void Detach()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce((transform.parent?.forward ?? Vector3.zero) * data.DroppingForce);
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
        }
        
        private void InitializeHeldVersion()
        {
            if (!data.HoldingPrefab) return;
            _heldVersion = Instantiate(data.HoldingPrefab, transform, true);
            _heldVersion!.transform.localPosition = data.HoldingPrefab.transform.position;
            _heldVersion.SetActive(false);
            DisableHeldVersionColliders();
        }

        private void DisableHeldVersionColliders()
        {
            var heldVersionColliders = _heldVersion?.GetComponents<Collider>();
            heldVersionColliders?.ToList().ForEach((col) => col.enabled = false);
            if (heldVersionColliders is { Length: > 0 }) Debug.LogError("Held prefab has colliders. They have been disabled.");
        }

        public void SetInteractionLocked(bool locked)
        {
            _isLocked = locked;
        }
        
        public override void OnHoverEnter(IInteractor interactor)
        {
            if (_isLocked)
                return;
            
            base.OnHoverEnter(interactor);

            var canInteract = _holder == null;
        }
        
        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}
