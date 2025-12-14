using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace cleaningcabin
{
    public class BroomMovementDetection : MonoBehaviour
    {
        private float _minBroomXPos = -0.3f;
        private float _maxBroomXPos = 0.01f;
        private float _broomXPos;
        
        
        private float _minBroomYPos = -0.1f;
        private float _maxBroomYPos = 0.3f;
        private float _broomYPos;
        
        private float _broomSpeed = 0.004f;

        private Transform _basePos;
        
        private Quaternion _baseRotation;
        private bool _isBaseRotation;
        // private bool _isBasePos;
            
        [SerializeField] private Transform broomModel;
        [SerializeField] private BroomScript broom;
        [SerializeField] private DirtInteractable di;
        void Awake()
        {
            if (broomModel != null)
            {
                _broomXPos = broomModel.localPosition.x;
            }
        }

        public void SetMiniGamePos()
        {
            broomModel.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            broomModel.localPosition = new Vector3(_broomXPos, _broomYPos, 2);
            // transform.rotation = new Quaternion(-90, 0, 0, 0);
            // transform.position = new Vector3(0, 0, 2);
        }
        
         // public void SetBroomRotation()
         // {
            //_baseRotation = broomModel.localRotation;//
            //_basePos = broomModel.transform;
            // _isBasePos = true;
            //_isBaseRotation = true;
        // }
        
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

            if (!di.isMiniGameActive) return;
            
            var delta = inputValue.Get<Vector2>();

            _broomXPos += delta.x * _broomSpeed;
            _broomXPos = Math.Clamp(_broomXPos, _minBroomXPos, _maxBroomXPos);

            _broomYPos += delta.y * _broomSpeed;
            _broomYPos = Math.Clamp(_broomYPos, _minBroomYPos, _maxBroomYPos);
            
            var broomPos = new Vector3(_broomXPos, _broomYPos, 
                broomModel.localPosition.z);
            broomModel.localPosition = broomPos;
        }
    }
}