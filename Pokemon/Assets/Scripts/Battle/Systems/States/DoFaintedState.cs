using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;

namespace Mfknudsen.Battle.Systems.States
{
    public class DoFaintedState : State
    {
        public DoFaintedState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            List<Pokemon> pokemons = master.GetFaintedPokemon();

            ConditionOversight checking = null;
            Coroutine condition = null;

            while (pokemons.Count > 0)
            {
                checking = pokemons[0].GetConditionOversight();
                condition = master.StartCoroutine(checking.CheckFaintedCondition());

                if (!checking.GetDone())
                    yield return 0;

                checking = null;
                condition = null;

                pokemons.RemoveAt(0);
            }
        }
    }
}