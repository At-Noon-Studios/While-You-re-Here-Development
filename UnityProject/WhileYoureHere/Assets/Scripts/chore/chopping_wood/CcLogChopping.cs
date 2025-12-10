using ScriptableObjects.chores;
using UnityEngine;

namespace chore.chopping_wood
{
    public class CcLogChopping : ChoreComponent
    {
        private readonly int _logID;
        private int _logCount;
        private readonly int _logAmountNeeded;

        private CcLogChopping(string name, string description, int logID, int logCount, int logAmountNeeded)
            : base(name, description)
        {
            _logID = logID;
            _logCount = logCount;
            _logAmountNeeded = logAmountNeeded;
            ComponentType = ChoreComponentType.LogChop;
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _logCount = 0;

            // ✅ Subscribe
            ChoreEvents.OnLogChopped += LogChopped;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();

            // ✅ Unsubscribe
            ChoreEvents.OnLogChopped -= LogChopped;

            TriggerComponentCompleted(this);
        }

        private void LogChopped(int logID)
        {
            if (_logID != logID)
                return;

            _logCount++;
            Debug.Log(
                $"Component {ComponentName}: Log Type {logID} chopped {_logCount}/{_logAmountNeeded}");

            if (_logCount >= _logAmountNeeded)
                MarkCompleted();
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            var local = soChoreComponent as ScriptableObjects.chores.ChoppingWood.CcChoppingWood;

            if (local == null)
            {
                Debug.LogError("Factory Error: Wrong SO type passed to CcLogPlacement.CreateFactory");
                return null;
            }

            return new CcLogChopping(
                local.componentName,
                local.description,
                local.logID,
                local.logCount,
                local.logAmountNeeded
            );
        }
    }
}