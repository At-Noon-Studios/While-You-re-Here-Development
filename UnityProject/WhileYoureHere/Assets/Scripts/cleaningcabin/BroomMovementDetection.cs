using System;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cleaningcabin
{
    public class BroomMovementDetection : HoldableObjectBehaviour
    {
        private float _minBroomXPos = -3f;
        private float _maxBroomXPos = 3f;
        private float _broomXPos;
        
        private float _minBroomYPos = -3f;
        private float _maxBroomYPos = 3f;
        private float _broomYPos;
        
        private float _broomSpeed = 0.004f;

        private Transform _basePos;
        
        private Quaternion _baseRotation;
        private bool _isBaseRotation;
            
        [SerializeField] private Transform broomModel;
        [SerializeField] private SweepingArea sweepingArea;


        public Vector3 _defaultLocalPos;
        public Quaternion _defaultLocalRot;
        public bool IsBroomBeingHeld => IsHeld;

        private new void Awake()
        {
            base.Awake();
            if (broomModel != null)
            {
                _broomXPos = broomModel.localPosition.x;
            }
            _defaultLocalPos = broomModel.localPosition;
            _defaultLocalRot = broomModel.localRotation;
            // _minBroomXPos = dirtInteractable.transform.localScale.x;
        }

        public void SetMiniGameStartPos()
        {
            // Kinda hardcoded values here, but they point the broom down from a top-down perspective
            broomModel.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            broomModel.localPosition = new Vector3(_broomXPos, _broomYPos, 2.5f);
        }

        public void ResetMiniGamePos()
        {
            // Also hardcoded, but it makes it stick to the player again
            broomModel.localPosition = new Vector3(0f, -1.5f, 0f);
            broomModel.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }
        
        public void OnClean(InputValue inputValue)
        {
            if (!sweepingArea.IsMiniGameActive)
            {
                Debug.Log("Minigame hasn't started!");
                return;
            }
            
            var delta = inputValue.Get<Vector2>();

            _broomXPos += delta.x * _broomSpeed;
            _broomXPos = Math.Clamp(_broomXPos, _minBroomXPos * sweepingArea.transform.localScale.x, _maxBroomXPos * sweepingArea.transform.localScale.x);

            _broomYPos += delta.y * _broomSpeed;
            _broomYPos = Math.Clamp(_broomYPos, _minBroomYPos * sweepingArea.transform.localScale.y, _maxBroomYPos * sweepingArea.transform.localScale.y);
            
            var broomPos = new Vector3(_broomXPos, _broomYPos, 
                broomModel.localPosition.z);
            broomModel.localPosition = broomPos;
        }
    }
}