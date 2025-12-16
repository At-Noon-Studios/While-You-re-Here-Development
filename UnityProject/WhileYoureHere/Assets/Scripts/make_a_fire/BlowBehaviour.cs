using System.Collections;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class BlowBehaviour : MonoBehaviour
    {
        [Header("Wind Configuration")]
        [SerializeField] private VisualEffect windEffect;
        [SerializeField] private AudioClip blowAudio;
        [SerializeField] private EventChannel blowEvent;
        [SerializeField] private EventChannel blowAllowedEvent;
        [SerializeField] private StoveBehaviour furnace;
        [SerializeField] private float blowDistance = 2.5f;

        private AudioSource _audioSource;
        private bool _canBlow;
        private Coroutine _windRoutine;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            windEffect = GetComponent<VisualEffect>();
        }

        private void OnEnable()
        {
            windEffect.Stop();
            
            blowEvent.OnRaise -= TryBlow;
            blowAllowedEvent.OnRaise -= EnableBlow;

            blowEvent.OnRaise += TryBlow;
            blowAllowedEvent.OnRaise += EnableBlow;
        }

        private void OnDisable()
        {
            blowEvent.OnRaise -= TryBlow;
            blowAllowedEvent.OnRaise -= EnableBlow;

            if (_windRoutine != null)
            {
                StopCoroutine(_windRoutine);
                _windRoutine = null;
            }
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
            if (!_canBlow)
                return;
            
            if (_windRoutine != null)
                StopCoroutine(_windRoutine);

            _windRoutine = StartCoroutine(WindTimer());

            var blowDirection = transform.TransformDirection(Vector3.up);
            Debug.DrawLine(transform.position, transform.position + blowDirection * blowDistance, Color.cyan, 1f);

            RaycastHit[] hits = Physics.RaycastAll(
                transform.position,
                blowDirection,
                blowDistance
            );

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                    continue;

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

            windEffect.Stop();
            _audioSource.Stop();

            _windRoutine = null;
        }
    }
}