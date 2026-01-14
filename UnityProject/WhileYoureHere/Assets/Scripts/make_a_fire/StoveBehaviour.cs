using System.Collections.Generic;
using System.Linq;
using chore;
using Interactable.Holdable;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace make_a_fire
{
    public class StoveBehaviour : MonoBehaviour
    {
        [Header("Burning Objects")] 
        [SerializeField] private HoldableObjectBehaviour newspaper;
        private List<GameObject> _logs;
        
        [Header("Fire Behaviour")] 
        [SerializeField] private ParticleSystem fireParticle;
        [SerializeField] private AudioClip matchStrike;
        [SerializeField] private AudioClip burningFire;
        [SerializeField] private AudioClip chargedFire;
        [SerializeField] private float chargedFireVolume;
        
        [Header("Blow Event")] 
        [SerializeField] private EventChannel blowAllowedEvent;
        
        private Light _fireLight;
        private float  _smallFireIntensity = 2.0f;
        private float _bigFireIntensity = 4.0f;
        
        private int _placedLogsCount;
        private bool _fireStarted;
        private AudioSource _audioSource;

        private void Awake()
        {
            PlayFireEffect(false);
            _audioSource = GetComponent<AudioSource>();
            _fireLight = GetComponent<Light>();
            _fireLight.intensity = 0.0f;
        }

        // private void Update()
        // {
        //     if (!newspaper.IsPlaced) return;
        //     _logs = GameObject.FindGameObjectsWithTag("HalfLog").ToList();
        //     var currentPlacedLogs = CountPlacedLogs();
        //     if (currentPlacedLogs > _placedLogsCount)
        //     {
        //         _placedLogsCount = currentPlacedLogs;
        //         switch (_fireStarted)
        //         {
        //             case false when _placedLogsCount == 1:
        //                 StartSmallFire();
        //                 break;
        //             case true when _placedLogsCount == 3:
        //                 blowAllowedEvent?.Raise();
        //                 break;
        //         }
        //     }
        // }

        void Update()
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                StartSmallFire();
            }
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                StartBigFire();
            }
        }
        
        private int CountPlacedLogs()
        {
            return _logs.Count(log => log.GetComponent<FurnacePlaceable>().IsPlaced);
        }

        private void StartSmallFire()
        {
            _fireStarted = true;
            SetFireLifetime(0.2f);
            
            fireParticle.gameObject.SetActive(true);
            fireParticle.Play();
            
            _audioSource.PlayOneShot(matchStrike);
            MakeFireSound();
            
            _fireLight.intensity = _smallFireIntensity;
        }

        public void StartBigFire()
        {
            SetFireLifetime(0.5f);
            _audioSource.PlayOneShot(chargedFire, chargedFireVolume);
           
            ChoreEvents.TriggerPaperPlacement();
            
            _fireLight.intensity = _bigFireIntensity;
        }

        private void MakeFireSound()
        {
            _audioSource.clip = burningFire;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void PlayFireEffect(bool status)
        {
            fireParticle.gameObject.SetActive(status);
            if (status) fireParticle.Play();
        }
        
        private void SetFireLifetime(float lifetime)
        {
            var main = fireParticle.main;
            main.startLifetime = lifetime;
        }
    }
}