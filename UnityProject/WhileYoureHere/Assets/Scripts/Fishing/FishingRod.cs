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
        private Animator _animator;
        private AnimatorStateInfo _state;
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
        
        private float _reelAnimTimer;
        private Quaternion[] _defaultRotations;
        private Transform[] _bones;

        [Header("General settings")]
        [SerializeField] private float castingForce;
        [SerializeField] private float distanceFromShoreForCatch;
        [SerializeField] private float minReelTime;
        [SerializeField] private float floaterSpeed;
        [SerializeField] private float fishStruggleBeforeEscape;
        [SerializeField] private int reelWhileStrugglePunishment;
        [SerializeField] private float _reelAnimHoldTime = 0.1f;

        [Header("Mouse settings")] 
        [SerializeField] private int minMouseToRightForCast;
        [SerializeField] private int minMouseUpForCast;
        [SerializeField] private int mouseMoveToCounterSteer;
        
        [Header("Fishing rod objects")]
        [SerializeField] private GameObject line;
        [SerializeField] private GameObject fishingRodTop;
        [SerializeField] private GameObject floaterPrefab;
        
        [Header("Caught fish voicelines")]
        [SerializeField] private List<AudioClip> success;
        [SerializeField] private List<AudioClip> fail;
        
        public static event Action<SoFish> OnFishCaught;
        public static void TriggerFishCaught(SoFish fish) => OnFishCaught?.Invoke(fish);
        
        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
          
            _bones = GetComponentsInChildren<Transform>();
            _defaultRotations = new Quaternion[_bones.Length];
            for (int i = 0; i < _bones.Length; i++) 
                _defaultRotations[i] = _bones[i].localRotation;
                
            _playerInput = player.GetComponent<PlayerInput>();
            _movementController = player.GetComponent<MovementController>();
            _cameraController = player.GetComponentInChildren<CameraController>();
            _audioSource = player.GetComponent<AudioSource>();
            _animator = GetComponentInChildren<Animator>();

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
        
        private void Update()
        {
            if (_animator == null) return;
            
            if (_holder == null)
            {
                _animator.enabled = false;
                return;
            }
            
            if (!_animator.enabled) 
            { 
                _animator.enabled = true;
                _animator.Play("No Rod", 0, 0f); 
            }

            _state = _animator.GetCurrentAnimatorStateInfo(0);
            if (_reelAnimTimer > 0f)
                _reelAnimTimer -= Time.deltaTime;

            bool isReelingAnim = _reelAnimTimer > 0f;
            _animator.SetBool("ReelInFish", isReelingAnim);
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
            Debug.Log("Now casting line !");
            _animator.SetTrigger("Castbitch");
            _isCasting = false;
            _isLineCast = true;
            _spawnedFloater = Instantiate(floaterPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
            _spawnedFloater.gameObject.GetComponent<Rigidbody>().AddForce(castingForce * _playerCamera.forward, ForceMode.Impulse);
            // _lineController.SetUpLine(new []{fishingRodTop.transform, _spawnedFloater.transform});
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
                _animator.SetTrigger("FishBite");
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
                _animator.SetTrigger("CatchFish");
                if (_state.IsName("Catch") && _state.normalizedTime >= 0.49)
                {
                    Instantiate(_caughtFish.fishPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
                    _audioSource.PlayOneShot(success[Random.Range(0, success.Count)]);
                    _caughtFish = null;
                    Debug.Log(_animator.GetCurrentAnimatorClipInfo(0));
                    StartCoroutine(ReturnLineAfterDelay(_animator.GetCurrentAnimatorClipInfo(0).Length));
                    _animator.ResetTrigger("CatchFish");
                }
                // if (_state.IsName("Catch") && _state.normalizedTime >= 1f) _animator.ResetTrigger("CatchFish");
                // Debug.Log(_state.normalizedTime);
            }
        }

        private IEnumerator ReturnLineAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnLine();
        }

        private void ReturnLine()
        {
            _isLineCast = false;
            line.SetActive(true);
            _lineController.SetUpLine(Array.Empty<Transform>());
            ResetPose();
            StopAllCoroutines();
            Destroy(_spawnedFloater);
            _fishEscapeCount = 0;
            _spawnedFloater = null;
            look -= UpdateMouseForCounterSteer;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
            _animator.Play("No Rod");
            _animator.ResetTrigger("CatchFish");
            // ResetBones();
        }

        private void OnReelFishingRod()
        {
            if (!_isLineCast) return;
            OnFishCaught -= ListenForFishCaught;
            
            _reelAnimTimer = _reelAnimHoldTime;

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
            _animator.Play("No Rod");
            _animator.ResetTrigger("CatchFish");
        }

        public override void Drop()
        {
            if (_isLineCast) return;
            base.Drop();
        }

        private void ResetBones()
        {
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].localRotation = _defaultRotations[i];
        }
    }
}