using System;
using Interactable.Holdable;
using player_controls;
using PlayerControls;
using ScriptableObjects.Events;
using ScriptableObjects.Fishing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fishing {
    public class FishingRod : HoldableObjectBehaviour
    {
        private PlayerInput _playerInput;
        private MovementController _movementController;
        private CameraController _cameraController;
        private bool _isCastPullPressed;
        private bool _isLineCast;
        private SoFish _caughtFish;
        private bool _isCasting;
        private LineController _lineController;
        private Vector3 _reelTargetPosition;
        private GameObject _spawnedFloater;
        private Vector2 _mouseDelta;
        private float _currentReelSpeed;

        public float castingForce;
        public float distanceFromShoreForCatch;
        public float minReelTime;

        
        public GameObject line;
        public GameObject fishingRodTop;
        public GameObject floaterPrefab;
        
        public static event Action<SoFish> OnFishCaught;
        public static void TriggerFishCaught(SoFish fish) => OnFishCaught?.Invoke(fish);
        
        private Action<Vector2> look;

        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            _playerInput = player.GetComponent<PlayerInput>();
            _movementController = player.GetComponent<MovementController>();
            _cameraController = player.GetComponentInChildren<CameraController>();

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
                if (!_isLineCast && !_isCasting)
                {
                    StartCast();
                }
            }
            else
            {
                if (_isCasting) UnChargeCast();
            }
        }

        private void StartCast()
        {
            _isCasting = true;
            _movementController.PauseMovement();
            _cameraController.PauseCameraMovement();
            // start hand towards corner
            _mouseDelta = new Vector2();
            look += UpdateMousePosition;
        }

        private void UpdateMousePosition(Vector2 mousePosition)
        {
            _mouseDelta += mousePosition;
            if (_mouseDelta.x > -800 || _mouseDelta.y < 600) return;
            look -= UpdateMousePosition;
            CastLine();
        }

        private void UnChargeCast()
        {
            //move hand back to original position
            _isCasting = false;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
            look -= UpdateMousePosition;
        }

        private void CastLine()
        {
            _isCasting = false;
            _isLineCast = true;
            _spawnedFloater = Instantiate(floaterPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
            _spawnedFloater.gameObject.GetComponent<Rigidbody>().AddForce(castingForce * _playerCamera.forward, ForceMode.Impulse);
            _lineController.SetUpLine(new []{fishingRodTop.transform, _spawnedFloater.transform});
            OnFishCaught += ListenForFishCaught;
            line.SetActive(false);
            ResetPose();
        }

        private void ListenForFishCaught(SoFish fish)
        {
            var camPosition = _playerCamera.transform.position;
            camPosition.y = _spawnedFloater.transform.position.y;
            camPosition.z = _spawnedFloater.transform.position.z;
            // needs work
            _reelTargetPosition = new Vector3(camPosition.x, camPosition.y, camPosition.z);
            
            
            
            _caughtFish = fish;
            _currentReelSpeed = Vector3.Distance(_reelTargetPosition, _spawnedFloater.transform.position) / minReelTime;
            Debug.Log(_currentReelSpeed);
        }

        private void ReelInFish()
        {
            var direction = Vector3.MoveTowards(_spawnedFloater.transform.position, _reelTargetPosition, _currentReelSpeed);
            _spawnedFloater.transform.position = direction;
            if (Vector3.Distance(_reelTargetPosition, _spawnedFloater.transform.position) <= distanceFromShoreForCatch )
            {
                //do catch animation here
                ReturnLine();
                Instantiate(_caughtFish.fishPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
                _caughtFish = null;
            }
        }

        private void ReturnLine()
        {
            _isLineCast = false;
            line.SetActive(true);
            _lineController.SetUpLine(Array.Empty<Transform>());
            Destroy(_spawnedFloater);
            _spawnedFloater = null;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
        }

        private void OnReelFishingRod()
        {
            if (!_isLineCast) return;
            OnFishCaught -= ListenForFishCaught;
            if (_caughtFish != null) 
            {
                ReelInFish();
            }
            else ReturnLine();
        }

        private void OnThrowFishingRod(InputValue mousePosition)
        {
            look?.Invoke(mousePosition.Get<Vector2>());
        }
    }
}