using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IPowerModify
    {
        public bool CanModify(Pokemon pokemon);

        public float Modification();
    }
}
