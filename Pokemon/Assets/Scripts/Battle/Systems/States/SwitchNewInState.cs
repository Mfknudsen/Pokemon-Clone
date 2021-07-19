#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Comunication;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.Systems.States
{
    public class SwitchNewInState : State
    {
        private readonly List<SwitchAction> switchActions;

        public SwitchNewInState(BattleMaster master, List<SwitchAction> switchActions) : base(master)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            foreach (SwitchAction switchAction in switchActions)
            {
                master.StartCoroutine(switchAction.Activate());

                while (!switchAction.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;
            }

            master.SetState(new RoundDoneState(master));
        }
    }
}