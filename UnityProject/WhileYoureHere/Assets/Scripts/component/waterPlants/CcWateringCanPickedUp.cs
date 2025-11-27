using chore;
using UnityEngine;

namespace component.waterPlants
{
    public class CcWateringCanPickedUp : ChoreComponent
    {
        private readonly int _wateringCanID;
        private int _wateringCanCount;
        private readonly int _wateringCanAmountNeeded;

        public CcWateringCanPickedUp(string name, string description, int wateringCanID, int wateringCanAmountNeeded) : base(name, description)
        {
            _wateringCanID = wateringCanID;
            _wateringCanAmountNeeded = wateringCanAmountNeeded;
            ComponentType = ChoreComponentType.WateringCanPickedUp;
        }

        // Is used in dictionary and Quest constructor
        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent) // Accept ScriptableObject data
        {
            SoCcWateringCanPickedUp localChoreComponent = soChoreComponent as SoCcWateringCanPickedUp; // Check type

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcWateringCanPickedUp.CreateFactory");
                return null;
            }

            return new CcWateringCanPickedUp( // Create runtime component
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.wateringCanID,
                localChoreComponent.wateringCanAmountNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _wateringCanCount = 0;
            ChoreEvents.OnWateringCanPickedUp += WateringCanPickedUp;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnWateringCanPickedUp -= WateringCanPickedUp;
            
            TriggerComponentCompleted(this);
        }

        private void WateringCanPickedUp(int wateringCanID)
        {
            if (!IsActive) return;
            if (_wateringCanID != wateringCanID)
                return;

            _wateringCanCount++;

            Debug.Log($"Component {ComponentName}: WateringCan Type {wateringCanID} was picked up {_wateringCanCount}/{_wateringCanAmountNeeded}");

            if (_wateringCanCount < _wateringCanAmountNeeded) return;
            
            MarkCompleted();
        }
    }
}
