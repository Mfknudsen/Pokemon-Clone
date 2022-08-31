#region Packages

using System.Collections;
using Runtime.Pokémon;
using Runtime.Systems;

#endregion

namespace Runtime.Battle.Systems.Static_Operations
{
    public class CatchPokemon : IOperation
    {
        private bool done;
        private readonly Pokemon pokemon;

        public CatchPokemon(Pokemon pokemon)
        {
            this.pokemon = pokemon;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            done = true;

            yield break;
        }

        public void End()
        {
        }
    }
}