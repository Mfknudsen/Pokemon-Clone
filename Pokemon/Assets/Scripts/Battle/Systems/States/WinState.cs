#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            yield break;

            master.EndBattle(true);
        }
    }
}