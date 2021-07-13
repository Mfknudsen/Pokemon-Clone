#region SDK

using System.Collections;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ComputerSelectNewState : State
    {
        public ComputerSelectNewState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            master.SetState(new RoundDoneState(master));
            
            yield break;
        }
    }
}
