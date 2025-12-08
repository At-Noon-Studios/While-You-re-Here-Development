using chore;
using ScriptableObjects.chores;
using UnityEngine;

namespace component.gardening
{
    public class CcWateringCanFilled : ChoreComponent
    {
        private readonly int _wateringCanID;
        private float _requiredFillAmount;

        public CcWateringCanFilled(string name, string description, int wateringCanID, float requiredFillAmount) : base(name, description)
        {
            _wateringCanID = wateringCanID;
            _requiredFillAmount = requiredFillAmount;
            ComponentType = ChoreComponentType.WateringCanFilled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            SoCcWateringCanFilled localChoreComponent = soChoreComponent as SoCcWateringCanFilled;
            
            if (localChoreComponent == null)
            {
                Debug.LogError("Factory Error: Wrong SO type passed to CcWateringCanFilled.CreateFactory");
                return null;
            }

            return new CcWateringCanFilled(
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.wateringCanID,
                localChoreComponent.requiredFillAmount);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnWateringCanFilled += WateringCanFilled;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnWateringCanFilled -= WateringCanFilled;
            
            TriggerComponentCompleted(this);
        }

        private void WateringCanFilled(int wateringCanID)
        {
            if (!IsActive) return;
            if (_wateringCanID != wateringCanID) return;
            
            Debug.Log($"Component {ComponentName}: Watering Can {wateringCanID} has been filled.");
            
            MarkCompleted();
        }
    }
}
