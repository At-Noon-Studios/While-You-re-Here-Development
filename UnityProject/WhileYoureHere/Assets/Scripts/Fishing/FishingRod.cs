using System;
using Interactable.Holdable;
using player_controls;
using PlayerControls;
using ScriptableObjects.Events;
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
        private GameObject _caughtFish;
        private bool _isCasting;
        private LineController _lineController;
        private int _currentReel;
        private GameObject _spawnedFloater;

        public float castingForce;
        
        [Header("Reeling line settings")] 
        public float reelSpeed;
        public float reelFramesBeforeCatch;
        
        public GameObject line;
        public GameObject fishingRodTop;
        public GameObject floaterPrefab;
        
        public static event Action<GameObject> OnFishCaught;
        public static void TriggerFishCaught(GameObject fish) => OnFishCaught?.Invoke(fish);
        
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
                if (!_isLineCast)
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
            look += UpdateMousePosition;
        }

        private void UpdateMousePosition(Vector2 mousePosition)
        {
            Debug.Log($"Mouse position: {mousePosition}");
            if (mousePosition.x > -400 && mousePosition.y > 200)
            {
                look -= UpdateMousePosition;
                CastLine();
            }
        }

        private void UnChargeCast()
        {
            //move hand back to original position
            _isCasting = false;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
            OnThrowFishingRod -= UpdateMousePosition;
        }

        private void CastLine()
        {
            _spawnedFloater = Instantiate(floaterPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
            _spawnedFloater.gameObject.GetComponent<Rigidbody>().AddForce(castingForce * _playerCamera.forward, ForceMode.Impulse);
            _lineController.SetUpLine(new []{fishingRodTop.transform, _spawnedFloater.transform});
            OnFishCaught += ListenForFishCaught;
            line.SetActive(false);
            ResetPose();
            _isLineCast = true;
            _isCasting = false;
        }

        private void ListenForFishCaught(GameObject fish)
        {
            //if fish not null, trigger some kind of effect
            _caughtFish = fish;
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

        private void OnReelFishingRod()
        {
            OnFishCaught -= ListenForFishCaught;
            if (_caughtFish != null) 
            {
                ReelInFish();
            }
            else ReturnLine();
        }

        private void OnThrowFishingRod(Vector2 mousePosition)
        {
            look.Invoke(mousePosition);
        }
    }
}