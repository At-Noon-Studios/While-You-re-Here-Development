using Interactable;
using ScriptableObjects.picking_up_objects;
using UnityEngine;

namespace picking_up_objects
{
    public class Pickable : InteractableBehaviour, IHeldObject, IInteractable
    {
        [SerializeField] private PickableData pickableData;
        [SerializeField] private Transform holdPoint;

        private bool _isHolding;
        private const float FollowSpeed = 5f;

        private Rigidbody _rb;
        private Transform _currentTargetPoint;

        private new void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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
            }
        }


        public override void OnHoverEnter()
        {
            print("hover enter");
            if (!_isHolding) base.OnHoverEnter();
        }

        public override void OnHoverExit()
        {
            print("hover exit");
            if (!_isHolding) base.OnHoverExit();
           
        }

        public float Weight => pickableData.weight;

        private void MoveTowardsHoldPoint()
        {
            var targetPosition = _currentTargetPoint.position;
            var direction = (targetPosition - transform.position);
            _rb.linearVelocity = direction * FollowSpeed;
        }

        public void Hold()
        {
            _isHolding = true;
            print("Maakt niet uit wat");
            base.OnHoverExit();
            _currentTargetPoint = holdPoint;
            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }

        public void Drop()
        {
            _isHolding = false;
            _rb.useGravity = true;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 0.05f;
        }

        public void Place(Transform targetPoint)
        {
            _isHolding = true;
            _currentTargetPoint = targetPoint;
            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }
    }
}