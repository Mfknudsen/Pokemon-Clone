#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Battle.Actions;
using Runtime.Communication;
using Runtime.Systems;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.Systems.States
{
    public class SwitchNewInState : State
    {
        private readonly List<SwitchAction> switchActions;

        public SwitchNewInState(BattleManager battleManager, List<SwitchAction> switchActions) : base(battleManager)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            foreach (SwitchAction switchAction in this.switchActions)
            {
                OperationsContainer container = new();
                container.Add(switchAction);
                OperationManager.instance.AddOperationsContainer(container);

                while (!switchAction.Done() || !ChatManager.instance.GetIsClear())
                    yield return null;
            }

            this.battleManager.SetState(new RoundDoneState(this.battleManager));
        }
    }
}