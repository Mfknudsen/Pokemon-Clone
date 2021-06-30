using System.Collections;

namespace Mfknudsen.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}
