#region SDK

using System.Collections;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleManager battleManager) : base(battleManager)
        {
        }

        public override IEnumerator Tick()
        {
            this. battleManager.EndBattle(false);
            
            yield break;
        }
    }
}