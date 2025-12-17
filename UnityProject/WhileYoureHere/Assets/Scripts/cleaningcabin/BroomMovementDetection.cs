using System;
using System.Collections.Generic;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cleaningcabin
{
    public class BroomMovementDetection : HoldableObjectBehaviour
    {
        [Header("Broom Configurations")]
        [SerializeField] private BroomConfig broomConfig;
        
        private SweepingArea _area;
        
        [Header("Broom being held")]
        public bool IsBroomBeingHeld => IsHeld;
        private SweepingArea _sweepableArea;
        
        private new void Awake()
        {
            base.Awake();
        }

        public void SetMiniGameStartPos()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.25f, _area.transform.localPosition.z * 2.0f);
            transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }

        public void ResetMiniGamePos()
        {
            transform.localPosition = new Vector3(0f, -1.5f, 0f);
            transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        public void OnClean(InputValue inputValue)
        {
            if (_area != null) 
            {
                _area.CleanArea(inputValue);
            }
        }

        public void SetActiveArea(SweepingArea area) => _area = area;
        
        public void ClearActiveArea() => _area = null;
    }
}