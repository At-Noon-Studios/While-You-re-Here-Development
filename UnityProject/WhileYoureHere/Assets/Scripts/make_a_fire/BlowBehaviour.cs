using System.Collections;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class BlowBehaviour : MonoBehaviour
    {
        [Header("Wind Configuration")] [SerializeField]
        private VisualEffect windEffect;

        [SerializeField] private AudioClip blowAudio;
        [SerializeField] private EventChannel blowEvent;
        [SerializeField] private EventChannel blowAllowedEvent;
        
        private AudioSource _audioSource;
        private bool _canBlow;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            windEffect = GetComponent<VisualEffect>();
        }

        private void OnEnable()
        {
            windEffect.Stop();
            blowEvent.OnRaise += TryBlow;
            blowAllowedEvent.OnRaise += EnableBlow;
        }

        private void OnDisable()
        {
            blowEvent.OnRaise -= TryBlow;
        }
        
        private void EnableBlow()
        {
            _canBlow = true;
        }
        
        private void TryBlow()
        {
            if (!_canBlow) return;
            StartCoroutine(WindTimer());
        }

        private IEnumerator WindTimer()
        {
            windEffect.Play();
            _audioSource.PlayOneShot(blowAudio);
            yield return new WaitForSeconds(2f);
            windEffect.Stop();
            _audioSource.Stop();
        }
    }
}