using System;
using Interactable;
using picking_up_objects;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [Header("Log placement settings")]
        [SerializeField] private Transform logPlaceholder;

        private GameObject _logObject;
        private bool _hasLog;
        
        public bool HasLog => _hasLog;
        public GameObject LogObject => _logObject;
        
        private void Awake()
        {
            base.Awake();
        }
        
        public override void Interact()
        {
            if (_hasLog) return;
            
            var heldController = GameObject.FindWithTag("Player")?.GetComponent<HeldObjectController>();
            var heldObject = heldController?.GetHeldObject();

            if (heldObject is Pickable pickableLog && pickableLog.CompareTag("Log"))
            {
                pickableLog.Place();
                pickableLog.transform.position = logPlaceholder.position;
                pickableLog.transform.rotation = logPlaceholder.rotation;
                
                var rb = pickableLog.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                
                pickableLog.transform.SetParent(logPlaceholder);
                
                _logObject = pickableLog.gameObject;
                _hasLog = true;
                Debug.Log($"Log Placed");
                
                heldController.ClearHeldObject();
            }
        }

        public void ClearLog()
        {
            _logObject = null;
            _hasLog = false;
            
            var rb = _logObject?.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
        
        protected override string InteractionText()
        {
            if (_hasLog) return "Stump (Occupied)";

            var heldController = GameObject.FindWithTag("Player")?.GetComponent<HeldObjectController>();
            var heldObject = heldController?.GetHeldObject();

            if (heldObject is Pickable pickableLog && pickableLog.CompareTag("Log"))
            {
                return "Place Log on Stump (E)";
            }

            return "Stump (Empty)";
        }
        
        public override void OnHoverEnter()
        {
            base.OnHoverEnter();
        }

        public override void OnHoverExit()
        {
            base.OnHoverExit();
        }
    }
}