using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactable.Holdable;
using Mono.Cecil.Cil;
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

        private int _placedLogsCount = 0;
        private bool _fireStarted = false;

        private void Awake()
        {
            PlayFireEffect(false);
        }

        private void Update()
        {
            if (!newspaper.IsPlaced) return;
            
            var currentPlacedLogs = CountPlacedLogs();


            if (currentPlacedLogs > _placedLogsCount)
            {
                var newLogs = currentPlacedLogs - _placedLogsCount;
                _placedLogsCount = currentPlacedLogs;

                switch (_fireStarted)
                {
                    case false when _placedLogsCount >= 1:
                        PlayFireEffect(true);
                        _fireStarted = true;
                        break;
                    case true:
                        IncreaseFireVelocity(newLogs);
                        break;
                }
            }
        }

        private int CountPlacedLogs()
        {
            return logs.Count(log => log.IsPlaced);
        }

        private void IncreaseFireVelocity(int amount)
        {
            var currentVelocity = fireParticle.GetVector3("FireVelocity");
            currentVelocity.y += 0.5f * amount;
            fireParticle.SetVector3("FireVelocity", currentVelocity);
        }

        private void PlayFireEffect(bool status)
        {
            fireParticle.gameObject.SetActive(status);
            if (status)
            {
                fireParticle.Play();
            }
        }
    }
}