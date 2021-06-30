using System.Collections;

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        public ActionState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}