using chore;

namespace component.making_tea
{
    public class CcKettleFilled : ChoreComponent
    {
        private float _requiredFill;

        public CcKettleFilled(string name, string desc, float requiredFill)
            : base(name, desc)
        {
            _requiredFill = requiredFill;
            ComponentType = ChoreComponentType.KettleFilled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent so)
        {
            var s = so as SoCcKettleFilled;
            if (s == null) return null;

            return new CcKettleFilled(s.componentName, s.description, s.requiredFill);
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