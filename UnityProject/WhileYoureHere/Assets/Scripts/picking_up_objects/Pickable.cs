using System;
using Interactable;
using ScriptableObjects.picking_up_objects;
using UnityEngine;

namespace picking_up_objects
{
    public class Pickable : InteractableBehaviour, IHeldObject, IInteractable
    {
        [SerializeField] private PickableData pickableData;
        [SerializeField] private Transform placeObjectPoint;
        private Transform _holdPoint;
        private HeldObjectController _heldObjectController;
        
        private bool _isHolding;
        private const float FollowSpeed = 5f;

        private Rigidbody _rb;
        private Transform _currentTargetPoint;
        
        private new void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            GameObject player = GameObject.FindWithTag("Player");
            _holdPoint = player.transform.Find("Main Camera/HoldObjectPoint");
            _heldObjectController = player.GetComponent<HeldObjectController>();
            base.Awake();
        }
        
        private void Update()
        {
            if (_isHolding && _currentTargetPoint)
            {
                MoveTowardsHoldPoint();
            }
        }

        public override void Interact()
        {
            if (!_isHolding)
            {
                Hold();

                _heldObjectController?.SetHeldObject(this);
            }
        }

        public override void OnHoverEnter()
        {
            if (!_isHolding)
            {
                base.OnHoverEnter();
            }
        }

        public override void OnHoverExit()
        {
            if (!_isHolding)
            {
                base.OnHoverExit();
            }
        }

        public float Weight => pickableData.weight;

        private void MoveTowardsHoldPoint()
        {
            var direction = _currentTargetPoint.position - transform.position;
            _rb.linearVelocity = direction * FollowSpeed;
        }

        private void Hold()
        {
            _isHolding = true;
            _currentTargetPoint = _holdPoint;
            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
            base.OnHoverExit();
        }

        public void Drop()
        {
            _isHolding = false;
            _rb.useGravity = true;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 0.05f;
        }

        public void Place()
        {
            _isHolding = false;
            if (placeObjectPoint) transform.position = placeObjectPoint.position;
            _rb.angularVelocity = Vector3.zero;
            _rb.useGravity = true;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }
    }
}