#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using Logger = Mfknudsen._Debug.Logger;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        private readonly SpotOversight spotOversight;
        private readonly OperationManager operationManager;

        public ActionState(BattleManager manager) : base(manager)
        {
            this.operationManager = OperationManager.instance;
            this.spotOversight = manager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Pokemon pokemon in this.spotOversight.GetSpots()
                         .Select(s =>
                             s.GetActivePokemon())
                         .Where(p =>
                             !p &&
                             !p.GetBattleAction() &&
                             p.GetConditionOversight().GetNonVolatileStatus() is not FaintedCondition))
            {
                #region Start Action

                BattleAction action = pokemon.GetBattleAction();

                Logger.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                OperationsContainer container = new(action);
                this.operationManager.AddOperationsContainer(container);

                while (!this.operationManager.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                #endregion

                #region Check Any Fainted

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Pokemon checkPokemon in this.spotOversight.GetSpots()
                             .Select(s =>
                                 s.GetActivePokemon())
                             .Where(p =>
                                 !p &&
                                 p.GetCurrentHealth() == 0))
                {
                    this.manager.SetPokemonFainted(checkPokemon);

                    if (checkPokemon.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition faintedCondition)
                    {
                        this.operationManager.AddOperationsContainer(new OperationsContainer(faintedCondition));

                        yield return null;

                        while (!faintedCondition.Done())
                            yield return null;
                    }

                    while (!this.operationManager.GetDone())
                        yield return null;
                }

                #endregion
            }

            if (this.manager.CheckTeamDefeated(true) ||
                this.manager.CheckTeamDefeated(false))
            {
                this.manager.SetState(new RoundDoneState(this.manager));

                yield break;
            }

            this.manager.SetState(new AfterConditionState(this.manager));
        }
    }
}