#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            yield break;
            
            master.EndBattle(false);
        }
    }
}