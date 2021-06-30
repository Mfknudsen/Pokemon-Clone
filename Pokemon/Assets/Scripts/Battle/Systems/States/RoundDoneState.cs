using System.Collections;

namespace Mfknudsen.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        public RoundDoneState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}