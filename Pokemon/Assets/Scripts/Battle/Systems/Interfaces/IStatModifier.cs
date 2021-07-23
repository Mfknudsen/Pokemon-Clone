using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IStatModifier
    {
        public bool CanModify(Pokemon pokemon, Stat stat);

        public float Modification();
    }
}