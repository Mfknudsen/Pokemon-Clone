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
        private readonly SpotOversight spotOversight;
        
        // ReSharper disable once IdentifierTypo
        public ActionState(BattleMaster master) : base(master)
        {
            pokemonsWithAction = new List<Pokemon>();

            spotOversight = master.GetSpotOversight();
            
            foreach (Spot spot in spotOversight.GetSpots())
            {
                // ReSharper disable once Unity.NoNullPropagation
                BattleAction action = spot?.GetActivePokemon()?.GetBattleAction();
                
                if(action is null) continue;
                
                pokemonsWithAction.Add(spot.GetActivePokemon());
            }
        }

        public override IEnumerator Tick()
        {
            bool faintedCheck = false;

            foreach (Pokemon pokemon in pokemonsWithAction)
            {
                // ReSharper disable once MergeSequentialChecks
                if (pokemon is null || pokemon.GetBattleAction() is null || pokemon.GetTurnDone())
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
                    Pokemon checkPokemon = sCheck?.GetActivePokemon();

                    if (checkPokemon is null) continue;

                    if (checkPokemon.GetCurrentHealth() == 0)
                        master.SetPokemonFainted(checkPokemon);

                    faintedCheck = true;
                }

                pokemonsWithAction.Remove(pokemon);

                if (!faintedCheck) continue;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Spot spot in spotOversight.GetSpots())
                {
                    // ReSharper disable once Unity.NoNullPropagation
                    if (spot?.GetActivePokemon()?.GetConditionOversight()?.GetNonVolatileStatus() is FaintedCondition)
                        pokemonsWithAction.Remove(spot.GetActivePokemon());
                }
                
                pokemon.SetTurnDone(true);

                break;
            }

            if (faintedCheck)
                master.SetState(new DoFaintedState(master));
            else
                master.SetState(new AfterConditionState(master));
        }
    }
}