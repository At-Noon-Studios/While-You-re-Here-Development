using chore;
using UnityEngine;
using System.Collections;
using Interactable.Concrete.ObjectHolder;
using Interactable.Holdable;

namespace making_tea
{
    public class Stove : MonoBehaviour
    {
        [Header("Heating Settings")]
        public float requiredFill = 0.2f;
        public float heatTime = 3f;

        [Header("Audio Clips")]
        public AudioClip boilingClip;
        public AudioClip whistleClip;

        [Header("Steam Settings")]
        public float steamStopDelay = 1.5f;

        private float _heatTimer;
        private Coroutine _steamRoutine;

        public bool isOn = true;

        private KettleFill _currentKettle;
        private ParticleSystem _steam;

        private AudioSource _boilingSource;
        private AudioSource _whistleSource;

        private ObjectHolder _holder;

        private void Awake()
        {
            var sources = GetComponents<AudioSource>();
            _boilingSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
            _whistleSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

            _boilingSource.loop = true;

            _holder = GetComponent<ObjectHolder>();
        }

        private void Start()
        {
            if (_holder == null) return;
            
            _holder.OnPlaced += HandlePlaced;
            _holder.OnRemoved += HandleRemoved;
        }
        
        private void HandlePlaced(IHoldableObject obj)
        {
            _currentKettle = (obj as MonoBehaviour)?.GetComponent<KettleFill>();
            if (_currentKettle == null) return;

            Debug.Log("Stove: kettle placed");

            _steam = _currentKettle.GetComponentInChildren<ParticleSystem>();
            _heatTimer = 0f;

            StartCoroutine(HeatRoutine());
        }

        private IEnumerator HeatRoutine()
        {
            if (!_boilingSource.isPlaying && boilingClip != null)
            {
                _boilingSource.clip = boilingClip;
                _boilingSource.Play();
            }

            while (_currentKettle != null && isOn)
            {
                _heatTimer += Time.deltaTime;

                if (_heatTimer >= heatTime)
                {
                    if (_steam != null && !_steam.isPlaying)
                        _steam.Play();

                    if (!_whistleSource.isPlaying && whistleClip != null)
                    {
                        _whistleSource.PlayOneShot(whistleClip);
                        ChoreEvents.TriggerWaterBoiled();
                    }
                }

                yield return null;
            }
        }

        private void HandleRemoved(IHoldableObject obj)
        {
            Debug.Log("Stove: kettle removed");
            StopEffects();
        }

        public void ToggleStove()
        {
            isOn = !isOn;
            if (!isOn)
                StopEffects();
        }

        private void StopEffects()
        {
            _heatTimer = 0f;

            if (_boilingSource.isPlaying)
                _boilingSource.Stop();

            if (_whistleSource.isPlaying)
                _whistleSource.Stop();

            if (_steam != null && _steam.isPlaying)
            {
                if (_steamRoutine != null)
                    StopCoroutine(_steamRoutine);
                _steamRoutine = StartCoroutine(SteamStopRoutine());
            }

            _currentKettle = null;
        }

        private IEnumerator SteamStopRoutine()
        {
            yield return new WaitForSeconds(steamStopDelay);

            if (_steam != null)
                _steam.Stop();
        }
    }
}
