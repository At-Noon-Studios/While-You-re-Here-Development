using chore;
using ScriptableObjects.chores;
using ScriptableObjects.chores.test;
using UnityEngine;

namespace component.test
{
    public class CcEnemyKilled : ChoreComponent
    {
        private readonly int _enemyID;
        private int _killCount;
        private readonly int _killsNeeded;

        public CcEnemyKilled(string name, string description, int enemyID, int killsNeeded) : base(name, description)
        {
            _enemyID = enemyID;
            _killsNeeded = killsNeeded;
            ComponentType = ChoreComponentType.EnemyKilled;
        }

        // Is used in dictionary and Chore constructor
        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent) // Accept ScriptableObject data
        {
            SoCcEnemyKilled localChoreComponent = soChoreComponent as SoCcEnemyKilled; // Check type

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcEnemyKilled.CreateFactory");
                return null;
            }

            return new CcEnemyKilled( // Create runtime component
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.enemyID,
                localChoreComponent.killsNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _killCount = 0;
            // Subscribe to enemy kill event
            ChoreEvents.OnEnemyKilled += EnemyKilled;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            // Unsubscribe from enemy kill event
            ChoreEvents.OnEnemyKilled -= EnemyKilled;
            
            TriggerComponentCompleted(this);
        }

        private void EnemyKilled(int enemyID)
        {
            if (_enemyID != enemyID)
                return;

            _killCount++;

            Debug.Log($"Component {ComponentName}: Enemy Type {enemyID} was killed {_killCount}/{_killsNeeded}");

            if (_killCount < _killsNeeded)
                return;

            MarkCompleted();
        }
    }
}
