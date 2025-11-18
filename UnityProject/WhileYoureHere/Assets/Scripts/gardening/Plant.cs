using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private GameObject _plantDry;
    [SerializeField] private GameObject _plantHealthy; 
    [SerializeField] private GameObject _plantWilt;

    [SerializeField] private float _plantStage = 0f;

    [SerializeField] private float _stageForDry = 0f;
    [SerializeField] private float _stageForHealthy = 1f; // from 1 to 8
    [SerializeField] private float _stageForWilt = 9f; // form 9 to 10
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckWatering();
    }

    public void CheckWatering()
    {
        if (_plantStage <= _stageForHealthy)
        {
            _plantDry.SetActive(true);
        }

        if (_plantStage >= _stageForHealthy && _plantStage < _stageForWilt)
        {
            _plantHealthy.SetActive(true);
        }

        if (_plantStage >= _stageForWilt)
        {
            _plantWilt.SetActive(true);
        }
    }
}
