using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IStatModifier
    {
        public float Modify(Pokemon pokemon, Stat stat);
    }
}