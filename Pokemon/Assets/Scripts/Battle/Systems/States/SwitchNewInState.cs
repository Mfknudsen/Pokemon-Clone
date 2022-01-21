#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Communication;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.Systems.States
{
    public class SwitchNewInState : State
    {
        private readonly List<SwitchAction> switchActions;

        public SwitchNewInState(BattleManager manager, List<SwitchAction> switchActions) : base(manager)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            foreach (SwitchAction switchAction in switchActions)
            {
                OperationsContainer container = new OperationsContainer();
                container.Add(switchAction);
                OperationManager.Instance.AddOperationsContainer(container);

                while (!switchAction.Done() || !ChatManager.instance.GetIsClear())
                    yield return null;
            }

            manager.SetState(new RoundDoneState(manager));
        }
    }
}