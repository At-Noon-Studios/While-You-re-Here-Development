using System;
using System.Collections;
using System.Collections.Generic;
using chore;
using time;
using UnityEngine;
using UnityEngine.Events;

namespace gamestate
{
    public class GamestateManager : MonoBehaviour
    {
        [Header("List with activities of this day")]
        [SerializeField] private List<Activity>  activities = new List<Activity>();
        
        private Activity _currentActivity;
        
        private static GamestateManager _instance;
        private TimeManager _timeManager;
        private ChoreManager _choreManager;
        private GameObject _player;
        private AudioSource _playerAudioSource;
        
        public int currentDay;
        public static Dictionary<string, bool> States = new Dictionary<string, bool>();

        private void Awake()
        {
            _instance = this;
        }

        private void OnValidate()
        {
            
        }

        private void Start()
        {
            GetComponent<TimeManager>();
            GetComponent<ChoreManager>();
            _player = GameObject.FindWithTag("Player");
            _playerAudioSource = _player.GetComponent<AudioSource>();
            _currentActivity = activities[0];
            HandleStartActivity();
        }

        private void Update()
        {
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
            foreach (var boolean in gameplayEvent.booleansToBeTrue)
            {
                if (!States.TryGetValue(boolean, out var value) || !value) return;
                HandleTrigger(gameplayEvent);
            }
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

        private void BooleanChange(string boolName, bool value)
        {
            States[boolName] = value;
        }

        private void SkyboxChange(int hourOfDay)
        {
            _timeManager.ChangeTime(currentDay, hourOfDay);
        }

        private void PlayCutscene()
        {
            // wont be implemented yet
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
                    PlayCutscene();
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

