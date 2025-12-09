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
        [Header("Fire Effect")] 
        [SerializeField] private VisualEffect fireParticle;
        
        [Header("Burning Objects")] 
        [SerializeField] private HoldableObjectBehaviour newspaper;
        [SerializeField] private List<HoldableObjectBehaviour> logs;

        [Header("Blow Event")] 
        [SerializeField] private EventChannel blowAllowedEvent;
        
        private int _placedLogsCount;
        private bool _fireStarted;
      
       
        
        private void Awake()
        {
            PlayFireEffect(false);
        }

        private void Update()
        {
            if (!newspaper.IsPlaced) 
                return;
            
            int currentPlacedLogs = CountPlacedLogs();

            if (currentPlacedLogs > _placedLogsCount)
            {
                _placedLogsCount = currentPlacedLogs;

                if (!_fireStarted && _placedLogsCount == 1)
                {
                    StartSmallFire();
                }
                else if (_fireStarted && _placedLogsCount == 3)
                {
                    // enable space press
                  
                    // if pressing space is enabled execute 
                    StartBigFire();
                    
                    blowAllowedEvent?.Raise();
                    
                    
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
            fireParticle.SetVector3("FireVelocity", new Vector3(0, 0.1f, 0));
            fireParticle.Play();
        }

        private void StartBigFire()
        {
            var velocity = new Vector3(0, 2f, 0);
            fireParticle.SetVector3("FireVelocity", velocity);
        }

        private void PlayFireEffect(bool status)
        {
            fireParticle.gameObject.SetActive(status);
            if (status)
                fireParticle.Play();
        }
    }
}