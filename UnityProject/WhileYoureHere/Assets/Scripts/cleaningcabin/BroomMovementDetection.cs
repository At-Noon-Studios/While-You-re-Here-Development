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

        private float _broomSpeed = 0.004f;
        [SerializeField] private float sweepingSpeed = 1f;

        [SerializeField] private Transform broomModel;
        [SerializeField] private SweepingArea sweepingArea;
        
        public bool IsBroomBeingHeld => IsHeld;

        [SerializeField] private GameObject sweepingAreaObj;
        private Material _sweepingColor;
        
        private Color _endingSweepingColor;
        private float _sweepingTimeInSeconds;

        private new void Awake()
        {
            base.Awake();
            if (broomModel != null)
            {
                _broomXPos = broomModel.localPosition.x;
            }
            _sweepingColor = sweepingAreaObj.GetComponent<MeshRenderer>().material;
            // sweepingAreaObj.GetComponent<MeshRenderer>().material = _sweepingColor;
            _endingSweepingColor = Color.white;
        }

        // public override void Interact(IInteractor interactor)
        // {
            // base.Interact(interactor);
            // Debug.Log("Monster");
        // }

        public void SetMiniGameStartPos()
        {
            broomModel.localPosition = new Vector3(_broomXPos, 0.25f, 2.5f);
            broomModel.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }

        public void ResetMiniGamePos()
        {
            broomModel.localPosition = new Vector3(0f, -1.5f, 0f);
            broomModel.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        public void OnClean(InputValue inputValue)
        {
            if (!sweepingArea.IsMiniGameActive) return;
            var delta = inputValue.Get<Vector2>();

            _broomXPos += delta.x * _broomSpeed;
            _broomXPos = Math.Clamp(_broomXPos, _minBroomXPos * sweepingArea.transform.localScale.x,
                _maxBroomXPos * sweepingArea.transform.localScale.x);
            
            broomModel.localPosition = new Vector3(_broomXPos, 0.25f, 2.5f);
            if (delta.x != 0 && delta.y != 0)
            {
                _sweepingTimeInSeconds += Time.deltaTime;
                _sweepingColor.color = Color.Lerp(_sweepingColor.color, _endingSweepingColor, sweepingSpeed * Time.deltaTime);
                if(_sweepingTimeInSeconds >= 0.5 && _sweepingTimeInSeconds <= 1)
                {
                    sweepingArea.EndSweepingMinigame();
                    _sweepingTimeInSeconds = 0;
                }
            }
        }
    }
}