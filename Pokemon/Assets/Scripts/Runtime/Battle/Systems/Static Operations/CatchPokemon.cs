#region Packages

using System.Collections;
using Runtime.Items.Pokeballs;
using Runtime.Pokémon;
using Runtime.Systems.Operation;
using UnityEngine.Events;

#endregion

namespace Runtime.Battle.Systems.Static_Operations
{
    public class CatchPokemon : IOperation
    {
        private bool done;
        private readonly Pokemon pokemon;
        private readonly Pokeball pokeball;
        private UnityEvent catchEvent;

        public CatchPokemon(Pokemon pokemon, Pokeball pokeball, UnityAction catchAction = null)
        {
            this.pokemon = pokemon;
            this.pokeball = pokeball;

            if (catchAction == null) return;

            this.catchEvent = new UnityEvent();
            this.catchEvent.AddListener(catchAction);
        }

        public bool IsOperationDone()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = true;

            yield break;
        }

        public void OperationEnd()
        {
        }
    }
}