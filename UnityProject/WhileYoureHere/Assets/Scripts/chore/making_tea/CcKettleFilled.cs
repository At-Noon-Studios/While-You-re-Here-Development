using ScriptableObjects.chores;

namespace chore.making_tea
{
    public class CcKettleFilled : ChoreComponent
    {

        private CcKettleFilled(string name, string desc)
            : base(name, desc)
        {
            ComponentType = ChoreComponentType.KettleFilled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent so)
        {
            var s = so as SoCcKettleFilled;
            if (s == null) return null;

            return new CcKettleFilled(s.componentName, s.description);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnKettleFilled += OnFill;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnKettleFilled -= OnFill;
        }

        private void OnFill()
        {
            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}