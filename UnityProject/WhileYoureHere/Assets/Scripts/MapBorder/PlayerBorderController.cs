using System.Collections;
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
    private Collider _collider;
    private AudioSource _audioSource;
    private bool _blockEffectRetrigger;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider =  GetComponent<Collider>();
        _player =  GameObject.Find("Player");
        _audioSource = _player.GetComponent<AudioSource>(); 
        GameObject.FindWithTag("BlurEffect").GetComponent<Volume>().profile.TryGet(out _vignette);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceVector = _player.transform.position - Physics.ClosestPoint(_player.transform.position, _collider, _collider.transform.position, _collider.transform.rotation);
        if (distanceVector.sqrMagnitude < distanceToTriggerEffects * distanceToTriggerEffects)
        {
            if (!_blockEffectRetrigger) StartCoroutine(PlayVoiceLine());
            _vignette.intensity.value = 1 - distanceVector.sqrMagnitude / (distanceToTriggerEffects * distanceToTriggerEffects);
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
