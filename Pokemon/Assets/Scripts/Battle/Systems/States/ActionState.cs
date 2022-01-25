#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        // ReSharper disable once IdentifierTypo
        private readonly SpotOversight spotOversight;
        private readonly OperationManager operationManager;

        // ReSharper disable once IdentifierTypo
        public ActionState(BattleManager manager) : base(manager)
        {
            operationManager = OperationManager.instance;
            spotOversight = manager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Pokemon pokemon in spotOversight.GetSpots()
                .Select(s => s.GetActivePokemon())
                .Where(p =>
                    p != null &&
                    p.GetBattleAction() != null &&
                    !(p.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition)))
            {
                #region Start Action

                BattleAction action = pokemon.GetBattleAction();

                BattleLog.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                OperationsContainer container = new OperationsContainer();
                container.Add(action);
                operationManager.AddOperationsContainer(container);

                while (!operationManager.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                #endregion

                #region Check Any Fainted

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Pokemon checkPokemon in spotOversight.GetSpots()
                    .Select(s =>
                        s.GetActivePokemon())
                    .Where(p =>
                        p != null &&
                        p.GetCurrentHealth() == 0))
                {
                    manager.SetPokemonFainted(checkPokemon);

                    FaintedCondition faintedCondition =
                        checkPokemon.GetConditionOversight().GetNonVolatileStatus() as FaintedCondition;

                    if (faintedCondition != null)
                    {
                        operationManager.AddOperationsContainer(new OperationsContainer(faintedCondition));

                        yield return null;

                        while (!faintedCondition.Done())
                            yield return null;
                    }

                    while (!operationManager.GetDone())
                        yield return null;
                }

                #endregion
            }

            if (manager.CheckTeamDefeated(true) ||
                manager.CheckTeamDefeated(false))
            {
                manager.SetState(new RoundDoneState(manager));

                yield break;
            }

            manager.SetState(new AfterConditionState(manager));
        }
    }
}