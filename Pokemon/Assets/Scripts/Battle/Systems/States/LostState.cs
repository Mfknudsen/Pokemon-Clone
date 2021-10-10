#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            manager.EndBattle(false);
            
            yield break;
        }
    }
}