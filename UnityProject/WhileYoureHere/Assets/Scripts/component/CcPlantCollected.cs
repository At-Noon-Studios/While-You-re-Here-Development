using chore;
using UnityEngine;

namespace component
{
    public class CcPlantCollected : ChoreComponent
    {
        private readonly int _plantsID;
        private int _plantsCount;
        private readonly int _plantsNeeded;

        public CcPlantCollected(string name, string description, int plantsID, int plantsNeeded) : base(name,
            description)
        {
            _plantsID = plantsID;
            _plantsCount = plantsNeeded;
            _plantsNeeded = 0;
            ComponentType = ChoreComponentType.PlantsCollected;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            var localChoreComponent = soChoreComponent as SoCCPlantsCollected; // Check type

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcPlantCollected.CreateFactory");
                return null;
            }

            return new CcPlantCollected(
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.plantsID,
                localChoreComponent.plantsNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnPlantsCollected += PlantCollected;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnPlantsCollected -= PlantCollected;
        }

        private void PlantCollected(int plantsID)
        {
            if (_plantsID != plantsID)
                return;

            _plantsCount++;

            Debug.Log($"{ComponentName}: Plant Type {plantsID} was collected {_plantsCount}/{_plantsNeeded}");

            if (_plantsCount < _plantsNeeded)
                return;

            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}
