using chore;

namespace component.making_tea
{
    public class CcTeabagAdded : ChoreComponent
    {
        public CcTeabagAdded(string n, string d) : base(n,d)
        {
            ComponentType = ChoreComponentType.TeabagAdded;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent so)
        {
            return new CcTeabagAdded(so.componentName, so.description);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnTeabagAdded += Done;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnTeabagAdded -= Done;
        }

        private void Done()
        {
            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}