#region SDK

using System.Collections;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            this. manager.EndBattle(false);
            
            yield break;
        }
    }
}