namespace Mfknudsen.AI.States
{
    public abstract class NpcState
    {
        protected NpcBase npcBase;

        protected NpcState(NpcBase npcBase)
        {
            this.npcBase = npcBase;
        }

        public abstract void Update();
    }
}