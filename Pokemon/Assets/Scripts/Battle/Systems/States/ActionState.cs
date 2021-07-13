#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen._Debug;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        // ReSharper disable once IdentifierTypo
        private readonly List<Pokemon> pokemonsWithAction;

        // ReSharper disable once IdentifierTypo
        public ActionState(BattleMaster master, List<Pokemon> pokemonsWithAction) : base(master)
        {
            this.pokemonsWithAction = pokemonsWithAction;
        }

        public override IEnumerator Tick()
        {
            bool faintedCheck = false;

            SpotOversight spotOversight = master.GetSpotOversight();

            foreach (Pokemon pokemon in pokemonsWithAction)
            {
                // ReSharper disable once MergeSequentialChecks
                if (pokemon is null || pokemon.GetBattleAction() is null)
                    continue;

                BattleAction action = pokemon.GetBattleAction();

                BattleLog.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                master.StartCoroutine(action.Activate());

                while (!action.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;

                yield return new WaitForSeconds(1);

                pokemon.SetBattleAction(null);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Spot sCheck in spotOversight.GetSpots())
                {
                    // ReSharper disable once Unity.NoNullPropagation
                    Pokemon p = sCheck?.GetActivePokemon();

                    if (p is null) continue;

                    if (p.GetCurrentHealth() == 0)
                        master.SetPokemonFainted(p);

                    faintedCheck = true;
                }

                pokemonsWithAction.Remove(pokemon);

                if (!faintedCheck) continue;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Spot spot in spotOversight.GetSpots())
                {
                    if (spot.GetActivePokemon().GetConditionOversight().GetNonVolatileStatus() is FaintedCondition)
                        pokemonsWithAction.Remove(spot.GetActivePokemon());
                }

                break;
            }

            if (faintedCheck)
                master.SetState(new DoFaintedState(master, pokemonsWithAction));
            else
                master.SetState(new PlayerSelectNewState(master));
        }
    }
}