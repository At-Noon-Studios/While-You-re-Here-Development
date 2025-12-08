using chore;
using ScriptableObjects.chores;
using UnityEngine;

namespace component.gardening
{
    public class CcPlantWatered : ChoreComponent
    {
        private readonly int _plantID;
        private int _wateringCount;
        private readonly int _plantStageNeeded;
    
        public CcPlantWatered(string name, string description, int plantID, int plantStageNeeded) : base(name, description)
        {
            _plantID = plantID;
            _plantStageNeeded = plantStageNeeded;
            ComponentType = ChoreComponentType.PlantWatered;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            SoCcPlantWatered localChoreComponent = soChoreComponent as SoCcPlantWatered;

            if (localChoreComponent == null)
            {
                Debug.LogError("Factory Error: Wrong SO type passed to CcPlantWatered.CreateFactory");
                return null;
            }
        
            return new CcPlantWatered(
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.plantID,
                localChoreComponent.plantStageNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _wateringCount = 0;
            ChoreEvents.OnPlantWatered += PlantWatered;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnPlantWatered -= PlantWatered;
            
            TriggerComponentCompleted(this);
        }

        private void PlantWatered(int plantID)
        {
            if (!IsActive) return;
            if (_plantID != plantID) return;
            
            _wateringCount++;
            
            Debug.Log($"Component {ComponentName}: Plant Type {plantID} was watered {_wateringCount}/{_plantStageNeeded}");

            if (_wateringCount < _plantStageNeeded) return;
            
            MarkCompleted();
        }
    }
}
