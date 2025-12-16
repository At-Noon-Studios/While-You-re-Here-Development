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
        [SerializeField] private float sweepingSpeed = 0.005f;
        
        // private Transform _basePos;
        // private Quaternion _baseRotation;
        // private bool _isBaseRotation;

        [SerializeField] private Transform broomModel;
        [SerializeField] private SweepingArea sweepingArea;
        
        public bool IsBroomBeingHeld => IsHeld;
        private Material _colorArea;

        [SerializeField] private GameObject sweepingAreaObj;
        private Color _startingColor;
        private Color _endingColor;
        
        [SerializeField] private Vector3 sweepingScale;   
        private new void Awake()
        {
            base.Awake();
            if (broomModel != null)
            {
                _broomXPos = broomModel.localPosition.x;
                _broomYPos = broomModel.localPosition.y;
            }
            _startingColor = sweepingAreaObj.GetComponent<MeshRenderer>().material.color;
            _colorArea.color = Color.black;
            _startingColor = _colorArea.color;
            _endingColor = Color.white;
        }

        public void SetMiniGameStartPos()
        {
            broomModel.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            broomModel.localPosition = new Vector3(_broomXPos, _broomYPos, 1.5f);
        }

        public void ResetMiniGamePos()
        {
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
            _broomXPos = Math.Clamp(_broomXPos, _minBroomXPos * sweepingArea.transform.localScale.x,
                _maxBroomXPos * sweepingArea.transform.localScale.x);

            _broomYPos += delta.y * _broomSpeed;
            _broomYPos = Math.Clamp(_broomYPos, _minBroomYPos * sweepingArea.transform.localScale.y,
                _maxBroomYPos * sweepingArea.transform.localScale.y);

            var broomPos = new Vector3(_broomXPos, _broomYPos,
                broomModel.localPosition.z);
            broomModel.localPosition = broomPos;


            if (delta.x != 0 && delta.y != 0)
            {
                _colorArea.color = Color.Lerp(_colorArea.color, _endingColor, sweepingSpeed * Time.deltaTime);
                // Debug.Log("is it 0 yet: " + sweepingSpeed * Time.deltaTime);
                if (sweepingSpeed * Time.deltaTime >= 1)
                {
                    sweepingArea.EndSweepingMinigame();
                }
            }
        }
    }
}