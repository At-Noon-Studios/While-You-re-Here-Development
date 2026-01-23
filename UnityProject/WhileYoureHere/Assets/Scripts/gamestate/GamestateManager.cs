using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using chore;
using player_controls;
using PlayerControls;
using scene_loading;
using ScriptableObjects.Gamestate;
using time;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace gamestate
{
    [RequireComponent(typeof(ChoreManager))]
    [RequireComponent(typeof(TimeManager))]
    public class GamestateManager : MonoBehaviour
    {
        public List<SoGamestateFlag> listOfFlags = new List<SoGamestateFlag>();
        
        [Header("List with activities of this day")]
        [SerializeField] private List<Activity>  activities = new List<Activity>();
        
        private Activity _currentActivity;
        
        private static GamestateManager _instance;
        private TimeManager _timeManager;
        private ChoreManager _choreManager;
        private GameObject _player;
        private AudioSource _playerAudioSource;
        private VideoPlayer _videoPlayer;
        private MovementController _movementController;
        private CameraController _cameraController;
        
        public int currentDay;
        
        private void Awake()
        {
            _instance = this;
            _timeManager = GetComponent<TimeManager>();
            _choreManager = GetComponent<ChoreManager>();
            _videoPlayer = GetComponent<VideoPlayer>();
            SetFlagsToDefault();
        }

        private void OnApplicationQuit()
        {
            SetFlagsToDefault();
        }

        private void SetFlagsToDefault()
        {
            foreach (var flag in listOfFlags)
            {
                flag.currentValue = flag.defaultValue;
            }
        }
        
        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _playerAudioSource = _player.GetComponent<AudioSource>();
            _movementController = _player.GetComponent<MovementController>();
            _cameraController = _player.GetComponentInChildren<CameraController>();
            _currentActivity = activities[0];
            SceneLoadingManager.OnFinish += HandleStartActivity;
        }

        private void Update()
        {
            if (!SceneLoadingManager.Finished) return;
            if (_currentActivity == null) return;
            foreach (var gameplayEvent in _currentActivity.events)
            {
                if (gameplayEvent.triggeredBy == TriggeredBy.OnChoresCompleted)
                {
                    CheckChoreCompletion(gameplayEvent);
                } else if (gameplayEvent.triggeredBy == TriggeredBy.BooleansToTrue)
                {
                    CheckBooleansTrue(gameplayEvent);
                }
            }

        }

        private void CheckBooleansTrue(GameplayEvent gameplayEvent)
        {
            foreach (var flag in gameplayEvent.booleansToBeTrue)
            {
                if (!flag.currentValue) return;
            }
            HandleTrigger(gameplayEvent);
        }

        public static GamestateManager GetInstance()
        {
            return _instance;
        }

        private void GoToNextActivity()
        {
            foreach (var gameplayEvent in _currentActivity.events )
            {
                if (gameplayEvent.triggeredBy is TriggeredBy.AfterFinishActivity)
                {
                    HandleTrigger(gameplayEvent);
                }
            }

            try
            {
                _currentActivity = activities[activities.IndexOf(_currentActivity) + 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError("No new activity was found: " + e.Message);
                return;
            }
            HandleStartActivity();
        }

        private void HandleStartActivity()
        {
            foreach (var gameplayEvent in _currentActivity.events )
            {
                if (gameplayEvent.triggeredBy is TriggeredBy.StartOfActivity)
                {
                    HandleTrigger(gameplayEvent);
                } else if (gameplayEvent.triggeredBy is TriggeredBy.AfterSetTime)
                {
                    StartCoroutine(ScheduleTrigger(gameplayEvent));
                }
            }
        }

        private void BooleanChange(SoGamestateFlag flag, bool value)
        {
            flag.currentValue = value;
        }

        private void SkyboxChange(int hourOfDay)
        {
            _timeManager.ChangeTime(currentDay, hourOfDay);
        }

        private void PlayCutscene(VideoClip clip)
        {
            _videoPlayer.clip = clip;
            _videoPlayer.Play();
            _movementController.PauseMovement();
            _cameraController.PauseCameraMovement();
            StartCoroutine(StopCutscene((float)clip.length));
        }

        private IEnumerator StopCutscene(float stopAfterSeconds)
        {
            yield return new WaitForSeconds(stopAfterSeconds);
            _videoPlayer.Stop();
            _movementController.ResumeMovement();
            _cameraController.ResumeCameraMovement();
        }

        private void PlayDialogue(AudioClip clip)
        {
            _playerAudioSource.PlayOneShot(clip);
        }

        private void InvokeCustomEvent(UnityEvent uEvent)
        {
            uEvent.Invoke();
        }

        private IEnumerator ScheduleTrigger(GameplayEvent gameplayEvent)
        {
            yield return new WaitForSeconds(gameplayEvent.triggerAfterSeconds);
            HandleTrigger(gameplayEvent);
        }
        
        private void CheckChoreCompletion(GameplayEvent gameplayEvent)
        {
            foreach (var chore in gameplayEvent.choresToComplete)
            {
                if (!_choreManager.CheckChoreCompletion(chore.id)) return;
            }
            HandleTrigger(gameplayEvent);
        }

        private void HandleTrigger(GameplayEvent gameplayEvent)
        {
            switch (gameplayEvent.type)
            {
                case GameplayEventType.BooleanChange:
                    BooleanChange(gameplayEvent.booleanToChange, gameplayEvent.newValue);
                    break;
                case GameplayEventType.SkyboxChange: 
                    SkyboxChange(gameplayEvent.hourOfDay);
                    break;
                case GameplayEventType.Cutscene:
                    PlayCutscene(gameplayEvent.cutsceneToPlay);
                    break;
                case GameplayEventType.Dialogue:
                    PlayDialogue(gameplayEvent.audioToPlay);
                    break;
                case GameplayEventType.ProgressToNextActivity:
                    GoToNextActivity();
                    break;
                case GameplayEventType.InvokeCustomEvent:
                    InvokeCustomEvent(gameplayEvent.eventToInvoke);
                    break;
            }
        }
    }
}

