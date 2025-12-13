using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace cleaningcabin
{
    public class BroomMovementDetection : MonoBehaviour
    {
        private float _minBroomPos = -55f;
        private float _maxBroomPos = 55f;
        private float _broomXPos;
        
        private float _broomYPos;
        
        private float _broomSpeed = 2.0f;

        private Transform _basePos;
        
        private Quaternion _baseRotation;
        private bool _isBaseRotation;
        private bool _isBasePos;
            
        [SerializeField] private Transform broomModel;
        [SerializeField] private BroomScript broom;
        void Awake()
        {
            if (broomModel != null)
            {
                _broomXPos = broomModel.localPosition.x;
                
            }
        }

         public void SetBroomRotation()
         {
            //_baseRotation = broomModel.localRotation;//
            //_basePos = broomModel.transform;
            _isBasePos = true;
            //_isBaseRotation = true;
        }
        public void OnClean(InputValue inputValue)
        {
            // if (!broom.IsBroomBeingHeld)
            // {
            //     Debug.Log("I'm not being held");
            //     return;
            // }
            // if (!_isBasePos)
            // {
            //     Debug.Log("I am not inside the broom base position");
            //     return;
            // }
            var delta = inputValue.Get<Vector2>();

            _broomXPos += delta.x * _broomSpeed;
            _broomXPos = Math.Clamp(_broomXPos, _minBroomPos, _maxBroomPos);

            var broomPos = new Vector3(_broomXPos, broomModel.localPosition.y, // Keep the current local Y position
                broomModel.localPosition.z);
            broomModel.localPosition = broomPos;
            //broomModel.localRotation = _baseRotation * Quaternion.Euler(0f, _broomYRotation, 0f);
        }
    }
}