using ScriptableObjects.picking_up_objects;
using UnityEngine;

namespace picking_up_objects
{
    public class Pickable : MonoBehaviour, IWeighted
    {
        [Header("Pickable Data")] [SerializeField]
        private PickableData pickableData;
        
        private const float FollowSpeed = 5f;
        private bool _isHolding;
        private Rigidbody _rb;
        private Transform _currentTargetPoint;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_isHolding && _currentTargetPoint)
            {
                MoveTowardsHoldPoint();
            }
        }
        
        public float Weight => pickableData.weight;
        
        private void MoveTowardsHoldPoint()
        {
            var targetPosition = _currentTargetPoint.position;
            var direction = (targetPosition - transform.position);
            _rb.linearVelocity = direction * FollowSpeed;
        }

        public void HoldObject(Transform holdPoint)
        {
            _isHolding = true;
            _currentTargetPoint = holdPoint;
            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }

        public void DropObject()
        {
            _isHolding = false;
            _currentTargetPoint = null;
            _rb.useGravity = true;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 0.05f;
        }

        public void PlaceObject(Transform targetPoint)
        {
            _isHolding = true;
            _currentTargetPoint = targetPoint;
            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }
        
  
    }
}