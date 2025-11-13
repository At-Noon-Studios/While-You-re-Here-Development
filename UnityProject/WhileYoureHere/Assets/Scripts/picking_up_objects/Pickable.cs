using UnityEngine;

namespace picking_up_objects
{
    public class Pickable : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 60f;

        private bool _isHeld;
        private Rigidbody _rb;
        private Transform _currentTargetPoint;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_isHeld && _currentTargetPoint)
            {
                MoveTowardsHoldPoint();
            }
        }
        
        private void MoveTowardsHoldPoint()
        {
            var targetPosition = _currentTargetPoint.position;
            var direction = (targetPosition - transform.position);
            _rb.linearVelocity = direction * followSpeed;
        }
        
        public void HoldObject(Transform holdPoint)
        {
            _isHeld = true;
            _currentTargetPoint = holdPoint;

            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }
        
        public void DropObject()
        {
            _isHeld = false;
            _currentTargetPoint = null;

            _rb.useGravity = true;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 0.05f;
        }
        
        public void PlaceObject(Transform targetPoint)
        {
            _isHeld = true;
            _currentTargetPoint = targetPoint;

            _rb.useGravity = false;
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
        }
    }
}
