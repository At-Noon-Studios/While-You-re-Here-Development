using System.Collections;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class BlowBehaviour : MonoBehaviour
    {
        [Header("Wind Configuration")] 
        [SerializeField] private ParticleSystem windEffect;
        [SerializeField] private AudioClip blowAudio;
        [SerializeField] private EventChannel blowEvent;
        [SerializeField] private EventChannel blowAllowedEvent;
        [SerializeField] private StoveBehaviour furnace;
        [SerializeField] private float blowDistance = 2.5f;
        
        private AudioSource _audioSource;
        private bool _canBlow;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (windEffect != null)
            {
                windEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            blowEvent.OnRaise += TryBlow;
            blowAllowedEvent.OnRaise += EnableBlow;
        }

        private void OnDisable()
        {
            blowEvent.OnRaise -= TryBlow;
            blowAllowedEvent.OnRaise -= EnableBlow;
        }

        private void EnableBlow()
        {
            _canBlow = true;
        }

        private void DisableBlow()
        {
            _canBlow = false;
        }

        private void TryBlow()
        {
            if (!_canBlow) return;
            StartCoroutine(WindTimer());

            var blowDirection = transform.TransformDirection(Vector3.forward);
            Debug.DrawLine(transform.position, transform.position + blowDirection * blowDistance, Color.cyan, 1f);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, blowDirection, blowDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player")) continue;
                if (hit.collider.CompareTag("Furnace"))
                {
                    furnace.StartBigFire();
                    DisableBlow();
                    break; 
                }
            }
        }

        private IEnumerator WindTimer()
        {
            windEffect.Play();
            _audioSource.PlayOneShot(blowAudio);
            yield return new WaitForSeconds(2f);
            windEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _audioSource.Stop();
        }
    }
}