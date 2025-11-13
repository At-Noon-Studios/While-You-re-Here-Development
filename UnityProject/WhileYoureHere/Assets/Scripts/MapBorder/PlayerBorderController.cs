using System.Collections;
using UnityEngine;

public class PlayerBorderController : MonoBehaviour
{
    [Header("Distance from border settings")] 
    [SerializeField] private int distanceToTriggerEffects;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip tooFarFromCabinVoiceLine;
    
    private GameObject _player;
    private Collider _collider;
    private bool _blockEffectRetrigger;
    private bool _fogApplied;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider =  GetComponent<Collider>();
        _player =  GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceVector = _player.transform.position - Physics.ClosestPoint(_player.transform.position, _collider, _collider.transform.position, _collider.transform.rotation);
        if (distanceVector.sqrMagnitude < distanceToTriggerEffects)
        {
            if (!_blockEffectRetrigger) StartCoroutine(ApplyBlurEffects());
        }
        else
        {
            _fogApplied = true;
        }
    }

    private IEnumerator ApplyBlurEffects()
    {
        _blockEffectRetrigger = true;
        _fogApplied = true;
        _player.TryGetComponent<AudioSource>(out var audioSource);
        audioSource.PlayOneShot(tooFarFromCabinVoiceLine);
        //apply blur somehow
        yield return new WaitForSeconds(5);
        _blockEffectRetrigger = false;
    }
}
