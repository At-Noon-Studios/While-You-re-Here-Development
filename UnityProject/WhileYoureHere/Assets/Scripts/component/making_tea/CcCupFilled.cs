using chore;
using ScriptableObjects.chores;

namespace component.making_tea
{
    public class CcCupFilled : ChoreComponent
    {
        private float _requiredFill;

        public CcCupFilled(string n, string d, float req) : base(n, d)
        {
            _requiredFill = req;
            ComponentType = ChoreComponentType.CupFilled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChore)
        {
            var so = soChore as SoCcCupFilled;
            return new CcCupFilled(so.componentName, so.description, so.requiredFill);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnCupFilled += Done;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnCupFilled -= Done;
        }

        void Done()
        {
            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}