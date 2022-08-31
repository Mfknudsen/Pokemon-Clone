using System.Collections;

namespace Runtime.Battle.Systems.States
{
    public abstract class State
    {
        protected readonly BattleManager manager;

        protected State(BattleManager manager)
        {
            this.manager = manager;
        }

        public abstract IEnumerator Tick();
    }
}