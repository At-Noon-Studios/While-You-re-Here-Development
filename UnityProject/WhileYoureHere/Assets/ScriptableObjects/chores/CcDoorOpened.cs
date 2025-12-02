using chore;
using UnityEngine;

namespace ScriptableObjects.chores
{
    public class CcDoorOpened : ChoreComponent
    {
        private readonly int _doorID;
        private int _doorCount;
        private readonly int _doorsOpenedNeeded;

        public CcDoorOpened(string name, string description, int doorID, int doorsOpenedNeeded) : base(name, description)
        {
            _doorID = doorID;
            _doorsOpenedNeeded = doorsOpenedNeeded;
            ComponentType = ChoreComponentType.DoorOpened;
        }

        // Is used in dictionary and Quest constructor
        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent) // Accept ScriptableObject data
        {
            SoCcDoorOpened localChoreComponent = soChoreComponent as SoCcDoorOpened; // Check type

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcAxePickedUp.CreateFactory");
                return null;
            }

            return new CcDoorOpened( // Create runtime component
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.doorID,
                localChoreComponent.doorsOpenedNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            Debug.Log("DoorChore ENABLED for doorID = " + _doorID);
            
            _doorCount = 0;
            ChoreEvents.OnDoorOpened += DoorOpened;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnDoorOpened -= DoorOpened;
            
            TriggerComponentCompleted(this);
        }

        private void DoorOpened(int doorID)
        {
            if (!IsActive) return;
            if (_doorID != doorID)
                return;

            _doorCount++;

            if (_doorCount < _doorsOpenedNeeded) return;
            
            MarkCompleted();
        }

    }
}
