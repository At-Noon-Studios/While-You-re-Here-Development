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
        // [Header("Sweeping References")]
        // [SerializeField] private Transform broomModel;
        // [SerializeField] private SweepingArea sweepingArea;
        
        [Header("Broom being held")]
        public bool IsBroomBeingHeld => IsHeld;

        // [Header("Sweepable Area")]
        // [SerializeField] private GameObject sweepingAreaObj;
        
        // private float _sweepingTimeInSeconds;
        // private float _broomXPos;
        // private Material _sweepingColor;
        // private Color _endingSweepingColor;
        private SweepingArea _sweepableArea;
        
        private new void Awake()
        {
            base.Awake();
            // if (broomModel != null)
            // {
            //     _broomXPos = broomModel.localPosition.x;
            // }
            // _sweepingColor = sweepingAreaObj.GetComponent<Renderer>().material;
            // _endingSweepingColor = Color.white;
        }

        public void SetMiniGameStartPos()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.25f, 2.5f);
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


        // public void OnClean(InputValue inputValue)
        // {
        //     if (!sweepingArea.IsMiniGameActive) return;
        //     var delta = inputValue.Get<Vector2>();
        //
        //     _broomXPos += delta.x * broomConfig.BroomSpeed;
        //     _broomXPos = Math.Clamp(_broomXPos, broomConfig.MinBroomXPos * sweepingArea.transform.localScale.x,
        //         broomConfig.MaxBroomXPos * sweepingArea.transform.localScale.x);
        //     
        //     broomModel.localPosition = new Vector3(_broomXPos, 0.25f, 2.5f);
        //     if (delta.x != 0)
        //     { 
        //         _sweepingTimeInSeconds += Time.deltaTime;
        //         _sweepingColor.color = Color.Lerp(_sweepingColor.color, _endingSweepingColor, Time.deltaTime * broomConfig.LerpSpeed);
        //         if(_sweepingTimeInSeconds >= 0.5 && _sweepingTimeInSeconds <= 1)
        //         {
        //             sweepingArea.EndSweepingMinigame();
        //             _sweepingTimeInSeconds = 0;
        //         }
        //     }
        // }
    }
}