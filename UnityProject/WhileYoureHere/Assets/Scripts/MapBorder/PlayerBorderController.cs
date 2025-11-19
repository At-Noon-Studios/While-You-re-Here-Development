using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerBorderController : MonoBehaviour
{
    [Header("Border effect settings")] 
    [SerializeField] private int distanceToTriggerEffects = 5;
    [SerializeField] private int timeBeforeRetrigger = 10;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip tooFarFromCabinVoiceLine;
    
    private GameObject _player;
    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;
    private Collider _collider;
    private AudioSource _audioSource;
    private bool _blockEffectRetrigger;
    private GameObject[] _borderObjects;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player =  GameObject.Find("Player");
        _audioSource = _player.GetComponent<AudioSource>();
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out _vignette);
        volume.profile.TryGet(out _colorAdjustments);
        _borderObjects = GameObject.FindGameObjectsWithTag("MapBorder");
    }

    // Update is called once per frame
    void Update()
    {
        var distances = new List<float>();
        foreach (GameObject borderObject in _borderObjects)
        {
            borderObject.TryGetComponent<Collider>(out  _collider);
            distances.Add((_player.transform.position - Physics.ClosestPoint(_player.transform.position, _collider, _collider.transform.position, _collider.transform.rotation)).sqrMagnitude);
        }

        var closestBorder = distances.Min();
        if (closestBorder < distanceToTriggerEffects * distanceToTriggerEffects)
        {
            if (!_blockEffectRetrigger) StartCoroutine(PlayVoiceLine());
            _vignette.intensity.value = Mathf.Clamp(1 - closestBorder / (distanceToTriggerEffects * distanceToTriggerEffects), 0f, 0.5f);
            _colorAdjustments.saturation.value = Mathf.Clamp((distanceToTriggerEffects * distanceToTriggerEffects - closestBorder) / (distanceToTriggerEffects * distanceToTriggerEffects) * -100, -100, 0);
        }
        else
        {
            _vignette.intensity.value = 0f;
            _colorAdjustments.saturation.value = 0;
        }
    }
    
    private IEnumerator PlayVoiceLine()
    {
        _blockEffectRetrigger = true;
        _audioSource.PlayOneShot(tooFarFromCabinVoiceLine);
        yield return new WaitForSeconds(timeBeforeRetrigger);
        _blockEffectRetrigger = false;
    }
}
