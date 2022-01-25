using System.Collections;
using Mfknudsen.Battle.Actions;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IOnMoveHit
    {
        public bool MultiHit(PokemonMove pokemonMove);

        public IEnumerator Operation();
    }
}
