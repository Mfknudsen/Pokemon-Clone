namespace Runtime.AI.Navigation
{
    public abstract class PathAction
    {
        public abstract bool PerformAction(UnitNavigationAgent agent);
    }
}