using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IFinalModifier
    {
        public float Modify(PokemonMove pokemonMove);
    }
}
