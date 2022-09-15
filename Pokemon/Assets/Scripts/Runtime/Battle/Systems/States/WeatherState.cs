#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Systems;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class WeatherState : State
    {
        public WeatherState(BattleManager battleManager) : base(battleManager)
        {
        }

        public override IEnumerator Tick()
        {
            OperationManager operationManager = OperationManager.instance;
            foreach (IOnTurnEnd onTurnEnd in this.battleManager.GetWeatherManager().GetWeatherWithInterface<IOnTurnEnd>()
                .Where(i => i is IOperation))
            {
                OperationsContainer container = new();
                container.Add((IOperation)onTurnEnd);
                operationManager.AddOperationsContainer(container);
            }

            this.battleManager.SetState(new RoundDoneState(this.battleManager));
            yield break;
        }
    }
}