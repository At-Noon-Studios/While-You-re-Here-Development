using ScriptableObjects.chores;

namespace chore.making_tea
{
    public class CcCupFilled : ChoreComponent
    {
        private CcCupFilled(string name, string desc)
            : base(name, desc)
        {
            ComponentType = ChoreComponentType.CupFilled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChore)
        {
            var so = soChore as SoCcCupFilled;
            return new CcCupFilled(so.componentName, so.description);
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

        private void Done()
        {
            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}