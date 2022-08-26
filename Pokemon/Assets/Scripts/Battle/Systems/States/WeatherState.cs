#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Interfaces;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class WeatherState : State
    {
        public WeatherState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            OperationManager operationManager = OperationManager.instance;
            foreach (IOnTurnEnd onTurnEnd in this.manager.GetWeatherManager().GetWeatherWithInterface<IOnTurnEnd>()
                .Where(i => i is IOperation))
            {
                OperationsContainer container = new();
                container.Add((IOperation)onTurnEnd);
                operationManager.AddOperationsContainer(container);
            }

            this.manager.SetState(new RoundDoneState(this.manager));
            yield break;
        }
    }
}