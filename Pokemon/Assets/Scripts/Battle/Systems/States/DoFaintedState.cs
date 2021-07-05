#region SDK

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class DoFaintedState : State
    {
        private readonly State toReturnTo;
        
        public DoFaintedState(BattleMaster master, State toReturnTo) : base(master)
        {
            this.toReturnTo = toReturnTo;
        }

        public override IEnumerator Tick()
        {
            // ReSharper disable once IdentifierTypo
            List<Pokemon> pokemons = master.GetFaintedPokemon();
            
            while (pokemons.Count > 0)
            {
                ConditionOversight checking = pokemons[0].GetConditionOversight();
                master.StartCoroutine(checking.CheckFaintedCondition());

                if (!checking.GetDone())
                    yield return 0;

                pokemons.RemoveAt(0);
            }
            
            master.SetState(toReturnTo);
        }
    }
}