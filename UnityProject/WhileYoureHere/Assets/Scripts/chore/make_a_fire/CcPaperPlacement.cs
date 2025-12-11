using chore.scavenging;
using ScriptableObjects.chores;
using ScriptableObjects.Chores.scavenging;
using UnityEngine;

namespace chore.make_a_fire
{
    public class CcPaperPlacement : ChoreComponent
    {
        private readonly int _paperID;
        private int _paperCount;
        private int _paperAmountNeeded;

        public CcPaperPlacement(string name, string description, int paperID, int paperAmountNeeded) :
            base(name, description)
        {
            _paperID = paperID;
            _paperAmountNeeded = paperAmountNeeded;
            ComponentType = ChoreComponentType.PaperPlacement;
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _paperCount = 0;

            ChoreEvents.OnPaperPlacement += PaperPaced;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            TriggerComponentCompleted(this);

            ChoreEvents.OnPaperPlacement -= PaperPaced;
        }

        private void PaperPaced()
        {
            //if (_paperID != paperID) return;
            //_paperCount++;
            // if (_paperCount >= _paperAmountNeeded) 
            MarkCompleted();
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            var localChoreComponent = soChoreComponent as SoCcPaperPlacement;

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcItemCollected.CreateFactory");
                return null;
            }

            return new CcPaperPlacement(
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.paperID,
                localChoreComponent.paperAmountNeeded);
        }
    }
}