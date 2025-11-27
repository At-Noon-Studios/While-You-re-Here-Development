using chore;

namespace component
{
    public class CcWaterBoiled : ChoreComponent
    {
        public CcWaterBoiled(string n, string d) : base(n, d)
        {
            ComponentType = ChoreComponentType.WaterBoiled;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent so)
        {
            return new CcWaterBoiled(so.componentName, so.description);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            ChoreEvents.OnWaterBoiled += Done;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnWaterBoiled -= Done;
        }

        void Done()
        {
            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}

