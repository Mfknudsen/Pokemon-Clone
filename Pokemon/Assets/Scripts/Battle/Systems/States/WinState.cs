#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            manager.EndBattle(true);
            
            yield break;
        }
    }
}