using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactable.Holdable;
using Mono.Cecil.Cil;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class StoveBehaviour : MonoBehaviour
    {
        [Header("Burning Objects")] 
        [SerializeField] private HoldableObjectBehaviour newspaper;
        [SerializeField] private List<HoldableObjectBehaviour> logs;
        
        [Header("Fire Behaviour")] 
        [SerializeField] private VisualEffect fireParticle;
        [SerializeField] private AudioClip matchStrike;
        [SerializeField] private AudioClip burningFire;
        [SerializeField] private AudioClip chargedFire;
        
        
        [Header("Blow Event")] 
        [SerializeField] private EventChannel blowAllowedEvent;
        
        private int _placedLogsCount;
        private bool _fireStarted;
        private AudioSource _audioSource;

        private void Awake()
        {
            PlayFireEffect(false);
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!newspaper.IsPlaced)
                return;

            var currentPlacedLogs = CountPlacedLogs();

            if (currentPlacedLogs > _placedLogsCount)
            {
                _placedLogsCount = currentPlacedLogs;

                switch (_fireStarted)
                {
                    case false when _placedLogsCount == 1:
                        StartSmallFire();
                        break;
                    case true when _placedLogsCount == 3:
                        blowAllowedEvent?.Raise();
                        break;
                }
            }
        }


        private int CountPlacedLogs()
        {
            return logs.Count(log => log.IsPlaced);
        }

        private void StartSmallFire()
        {
            _fireStarted = true;
            fireParticle.gameObject.SetActive(true);
            
            _audioSource.PlayOneShot(matchStrike);
            MakeFireSound();
            fireParticle.SetVector3("FireVelocity", new Vector3(0, 0.1f, 0));
            fireParticle.Play();
        }

        public void StartBigFire()
        {
            _audioSource.PlayOneShot(chargedFire);
            fireParticle.SetVector3("FireVelocity", new Vector3(0, 2f, 0));
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
    }
}