using chore;
using ScriptableObjects;
using UnityEngine;

namespace gardening
{
    public class PlantObject : MonoBehaviour
    {
        [Header("Plant settings")]
        [SerializeField] private SoPlant plant;
        [SerializeField] private int plantID;
        [SerializeField] private ParticleSystem wateringParticles;
        
        public SoPlant PlantData => plant;
        public int CurrentStage => _currentStage;
        
        private GameObject _currentPlant;
        private int _currentStage;
        private float _currentWaterTime = 0f;
        private bool _isWatering = false;
        private bool _isTriggered = false;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (plant == null || plant.plantPrefabs.Count == 0)
            {
                Debug.LogError("PlantObject: Plant prefabs not assigned!");
                return;
            }

            SpawnCurrentStage();
            
            Debug.Log($"max stages: {plant.MaxStage}");
            Debug.Log($"plant: {_currentPlant}");
            Debug.Log($"stage: {_currentStage}");
        }

        void Update()
        {
            if (_isWatering && _currentStage < plant.MaxStage)
            {
                _currentWaterTime += Time.deltaTime * 10;
                Debug.Log(_currentWaterTime);

                if (_currentWaterTime >= plant.wateringTime)
                {
                    Grow();
                    _currentWaterTime = 0;
                }
            }
        }

        public void SpawnCurrentStage()
        {
            GameObject prefabPlant = plant.GetPlantByStage(_currentStage);
            if (prefabPlant == null) return;
            
            _currentPlant = Instantiate(prefabPlant, transform.position, transform.rotation);
        }
        
        public void StartWatering()
        {
            _isWatering = true;
        }

        public void LateUpdate()
        {
            _isWatering = false;
        }

        public void Grow()
        {
            _currentStage++;
            
            GameObject nextPrefab = plant.GetPlantByStage(_currentStage);

            if (nextPrefab == null)
            {
                Debug.Log($"No prefab found for stage {_currentStage}");
                return;
            }
            
            if (_currentStage < plant.MaxStage && _currentPlant != null)
            {
                Destroy(_currentPlant);
            }
            
            if (!_isTriggered)
            {
                _isTriggered = true;
                ChoreEvents.TriggerPlantWatered(plantID);
            }
            
            _currentPlant = Instantiate(nextPrefab, transform.position, transform.rotation);
            Debug.Log(_currentPlant);
        }
    }
}
