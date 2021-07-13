#region SDK

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class DoFaintedState : State
    {
        // ReSharper disable once IdentifierTypo
        private readonly List<Pokemon> pokemonsWithAction;

        // ReSharper disable once IdentifierTypo
        public DoFaintedState(BattleMaster master, List<Pokemon> pokemonsWithAction) : base(master)
        {
            this.pokemonsWithAction = pokemonsWithAction;
        }

        public override IEnumerator Tick()
        {
            // ReSharper disable once IdentifierTypo
            // ReSharper disable once Unity.NoNullPropagation
            List<Pokemon> pokemons = (master.GetSpotOversight()
                .GetSpots()
                .Where(spot =>
                    spot?.GetActivePokemon()?.GetConditionOversight()?.GetNonVolatileStatus() is FaintedCondition)
                .Select(spot => spot.GetActivePokemon())).ToList();

            while (pokemons.Count > 0)
            {
                ConditionOversight checking = pokemons[0].GetConditionOversight();

                if (checking is null) continue;

                master.StartCoroutine(checking.CheckFaintedCondition());

                while (!checking.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;

                pokemons.RemoveAt(0);
            }

            master.SetState(new ActionState(master, pokemonsWithAction));
        }
    }
}