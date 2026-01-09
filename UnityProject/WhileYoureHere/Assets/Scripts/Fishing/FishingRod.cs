using System;
using System.Collections;
using System.Collections.Generic;
using Interactable.Holdable;
using player_controls;
using PlayerControls;
using ScriptableObjects.Fishing;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Fishing {
    public class FishingRod : HoldableObjectBehaviour
    {
        private PlayerInput _playerInput;
        private MovementController _movementController;
        private CameraController _cameraController;
        private AudioSource _audioSource;
        private LineController _lineController;
        private GameObject _spawnedFloater;
        private SoFish _caughtFish;
        private Action<Vector2> look;
        
        private bool _isCastPullPressed;
        private bool _isLineCast;
        private bool _isCasting;
        private bool _allowReeling;
        
        private Vector3 _reelTargetPosition;
        private Vector3 _currentFloaterMovement;
        private Vector3 _sidewaysMovementToDo;
        private Vector2 _mouseDelta;
        private float _currentReelSpeed;
        private float _fishEscapeCount;
        private Vector3 _directionOfFloater;
        private readonly int[] _directions = {-1, 1};

        [Header("General settings")]
        public float castingForce;
        public float distanceFromShoreForCatch;
        public float minReelTime;
        public float floaterSpeed;
        public float fishStruggleBeforeEscape;
        public int reelWhileStrugglePunishment;

        [Header("Mouse settings")] 
        public int minMouseToRightForCast;
        public int minMouseUpForCast;
        public int mouseMoveToCounterSteer;
        
        [Header("Fishing rod objects")]
        public GameObject line;
        public GameObject fishingRodTop;
        public GameObject floaterPrefab;
        
        [Header("Caught fish voicelines")]
        public List<AudioClip> success;
        public List<AudioClip> fail;
        
        public static event Action<SoFish> OnFishCaught;
        public static void TriggerFishCaught(SoFish fish) => OnFishCaught?.Invoke(fish);
        
        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            _playerInput = player.GetComponent<PlayerInput>();
            _movementController = player.GetComponent<MovementController>();
            _cameraController = player.GetComponentInChildren<CameraController>();
            _audioSource = player.GetComponent<AudioSource>();

            _playerInput.actions["CastPullFishingRod"].performed += ctx => _isCastPullPressed = true;
            _playerInput.actions["CastPullFishingRod"].canceled += ctx => _isCastPullPressed = false;
            _lineController = GetComponent<LineController>();
        }

        private void FixedUpdate()
        {
            if (_holder == null) return;
            if (_isCastPullPressed)
            {
                if (!_isLineCast && !_isCasting) StartCast();
                if (_caughtFish != null  && look == null) look += UpdateMouseForCounterSteer;
            }
            else
            {
                if (_isCasting) UnChargeCast();
                if (_caughtFish != null) look -= UpdateMouseForCounterSteer;
            }
            if (_caughtFish == null) return;
            _currentFloaterMovement = _spawnedFloater.transform.position;
            MoveFloater();
            Debug.Log(IsCounterSteering() +" : " +_mouseDelta.x + " : " + _directionOfFloater.x);
             if (!IsCounterSteering())
             {
                 _fishEscapeCount++;
                 if (_fishEscapeCount >= fishStruggleBeforeEscape) FishEscape();
             }
             else _fishEscapeCount = Mathf.Max(0, _fishEscapeCount - 1);
        }

        private void StartCast()
        {
            _isCasting = true;
            _movementController.PauseMovement();
            _cameraController.PauseCameraMovement();
            // start hand towards corner
            _mouseDelta = new Vector2();
            look += UpdateMouseForCast;
        }

        private void UpdateMouseForCast(Vector2 mousePosition)
        {
            _mouseDelta += mousePosition;
            if (_mouseDelta.x > minMouseToRightForCast || _mouseDelta.y < minMouseUpForCast) return;
            look -= UpdateMouseForCast;
            CastLine();
        }

        private void UpdateMouseForCounterSteer(Vector2 mousePosition)
        {
            _mouseDelta.x = Mathf.Clamp(_mouseDelta.x + mousePosition.x, -mouseMoveToCounterSteer * 2, mouseMoveToCounterSteer * 2);
        }

        private bool IsCounterSteering()
        {
            if (_directionOfFloater.x < 0) return _mouseDelta.x > mouseMoveToCounterSteer * -_directionOfFloater.x;
            if (_directionOfFloater.x > 0) return _mouseDelta.x < mouseMoveToCounterSteer * -_directionOfFloater.x;
            return _mouseDelta.x > -mouseMoveToCounterSteer && _mouseDelta.x < mouseMoveToCounterSteer;
        }

        private void UnChargeCast()
        {
            //move hand back to original position
            _isCasting = false;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
            look -= UpdateMouseForCast;
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
            if (Physics.Raycast(_spawnedFloater.transform.position, camPosition - _spawnedFloater.transform.position, out var hit))
            {
                _reelTargetPosition = hit.point;
                _caughtFish = fish;
                _currentReelSpeed = Vector3.Distance(_reelTargetPosition, _spawnedFloater.transform.position) / minReelTime;
                _mouseDelta = new Vector2();
                look += UpdateMouseForCounterSteer;
                _audioSource.PlayOneShot(success[Random.Range(0, success.Count)]);
                StartCoroutine(FishStruggle());
            }
            else
            {
                _caughtFish = null;
            }
        }

        private void ReelInFish()
        {
            if (!_allowReeling)
            {
                _fishEscapeCount += reelWhileStrugglePunishment;
                return;
            }

            //_currentFloaterMovement += (_reelTargetPosition - _spawnedFloater.transform.position) * _currentReelSpeed;
            var direction = Vector3.MoveTowards(_spawnedFloater.transform.position, _reelTargetPosition, _currentReelSpeed);
            _spawnedFloater.transform.position = direction;
        }

        private void MoveFloater()
        {
            var mvm = floaterSpeed * Time.deltaTime * _sidewaysMovementToDo;
            _currentFloaterMovement += mvm;
            _sidewaysMovementToDo -= mvm;
            FloaterController.TriggerFloaterMove(_currentFloaterMovement);
            if (Vector3.Distance(_reelTargetPosition, _spawnedFloater.transform.position) <= distanceFromShoreForCatch)
            {
                //do catch animation here
                ReturnLine();
                Instantiate(_caughtFish.fishPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
                _audioSource.PlayOneShot(success[Random.Range(0, success.Count)]);
                _caughtFish = null;
            }
        }

        private void ReturnLine()
        {
            _isLineCast = false;
            line.SetActive(true);
            _lineController.SetUpLine(Array.Empty<Transform>());
            StopAllCoroutines();
            Destroy(_spawnedFloater);
            _fishEscapeCount = 0;
            _spawnedFloater = null;
            look -= UpdateMouseForCounterSteer;
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

        private IEnumerator FishStruggle()
        {
            _allowReeling = false;
            FloaterController.TriggerFishSplashing(true);
            _directionOfFloater = new Vector3(_directions[Random.Range(0, _directions.Length)] * _caughtFish.fishCatchDifficulty.sidewaysMovement, 0, 0);
            _sidewaysMovementToDo = _directionOfFloater;
            yield return new WaitForSeconds(_caughtFish.fishCatchDifficulty.splashDuration);
            StartCoroutine(FishRelax());
        }

        private IEnumerator FishRelax()
        {
            _allowReeling = true;
            FloaterController.TriggerFishSplashing(false);
            _sidewaysMovementToDo = _directionOfFloater * -1;
            _directionOfFloater = new Vector3();
            yield return new WaitForSeconds(_caughtFish.fishCatchDifficulty.splashInterval);
            StartCoroutine(FishStruggle());
        }

        private void FishEscape()
        {
            _caughtFish = null;
            OnFishCaught -= ListenForFishCaught;
            _audioSource.PlayOneShot(fail[Random.Range(0, fail.Count)]);
            ReturnLine();
        }

        public override void Drop()
        {
            if (_isLineCast) return;
            base.Drop();
        }
    }
}