using System;
using System.Collections;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fishing {
    public class FishingRod : HoldableObjectBehaviour
    {
        private PlayerInput _playerInput;
        private bool _isCastPullPressed;
        private bool _isLineCast;
        private GameObject _caughtFish;
        private bool _isCasting;
        private float _castingForce;
        private LineController _lineController;
        private int _currentCharge;
        private int _currentReel;
        private GameObject _spawnedFloater;

        [Header("Casting line settings")]
        public float castingForceMultiplier;
        public float chargeCastTiltMultiplier;
        public int framesForMaxCharge;
        public int framesForMinCharge;

        [Header("Reeling line settings")] 
        public float reelSpeed;
        public float reelFramesBeforeCatch;
        
        public GameObject line;
        public GameObject fishingRodTop;
        public GameObject floaterPrefab;
        
        public static event Action<GameObject> OnFishCaught;
        public static void TriggerFishCaught(GameObject fish) => OnFishCaught?.Invoke(fish);


        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            _playerInput = player.GetComponent<PlayerInput>();

            if (_playerInput == null) return;
            _playerInput.actions["CastPullFishingRod"].performed += ctx => _isCastPullPressed = true;
            _playerInput.actions["CastPullFishingRod"].canceled += ctx => _isCastPullPressed = false;
            _lineController = GetComponent<LineController>();
        }

        private void Update()
        {
            if (_holder == null) return;
            if (_isCastPullPressed)
            {
                if (!_isLineCast)
                {
                    ChargeCast();
                }
                else PullLine();
            }
            else
            {
                if (_isCasting) UnChargeCast();
            }
        }

        private void ChargeCast()
        {
            if (_currentCharge >= framesForMaxCharge) return;
            _currentCharge++;
            _isCasting = true;
            _castingForce += castingForceMultiplier * Time.deltaTime;
            transform.Rotate(-chargeCastTiltMultiplier, 0, 0);
        }

        private void UnChargeCast()
        {
            if (_currentCharge > framesForMinCharge)
            {
                CastLine();
            } else if (_currentCharge <= 0)
            {
                _isCasting = false;
                _currentCharge = 0;
                _castingForce = 0;
            } else {
                _currentCharge--;
                _castingForce -= castingForceMultiplier * Time.deltaTime;
                transform.Rotate(chargeCastTiltMultiplier, 0, 0);
            }
        }
        

        private void CastLine()
        {
            _spawnedFloater = Instantiate(floaterPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
            _spawnedFloater.gameObject.GetComponent<Rigidbody>().AddForce(_castingForce * _playerCamera.forward, ForceMode.Impulse);
            _lineController.SetUpLine(new []{fishingRodTop.transform, _spawnedFloater.transform});
            OnFishCaught += ListenForFishCaught;
            line.SetActive(false);
            ResetPose();
            _isLineCast = true;
            _isCasting = false;
            _castingForce = 0;
            _currentCharge = 0;
        }

        private void ListenForFishCaught(GameObject fish)
        {
            //if fish not null, trigger some kind of effect
            _caughtFish = fish;
        }

        private void PullLine()
        {
            OnFishCaught -= ListenForFishCaught;
            if (_caughtFish != null) 
            {
                ReelInFish();
            }
            else ReturnLine();
        }

        private void ReelInFish()
        {
            if (_currentReel >= reelFramesBeforeCatch)
            {
                _currentReel = 0;
                ReturnLine();
                Instantiate(_caughtFish, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
                _caughtFish = null;
            }
            var camPosition = _playerCamera.transform.position;
            camPosition.y = _spawnedFloater.transform.position.y;
            var direction = Vector3.MoveTowards(_spawnedFloater.transform.position, camPosition, reelSpeed * Time.deltaTime);
            _spawnedFloater.transform.position = direction;
            _currentReel++;
        }

        private void ReturnLine()
        {
            //unsub from mouse listen until cancel pressed
            line.SetActive(true);
            _lineController.SetUpLine(Array.Empty<Transform>());
            _isLineCast = false;
            Destroy(_spawnedFloater);
            _spawnedFloater = null;
        }
    }
}