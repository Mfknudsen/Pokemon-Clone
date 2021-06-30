using System.Collections;

namespace Mfknudsen.Battle.Systems.States
{
    public abstract class State
    {
        protected readonly BattleMaster master;

        protected State(BattleMaster master)
        {
            this.master = master;
        }

        public abstract IEnumerator Tick();
    }
}