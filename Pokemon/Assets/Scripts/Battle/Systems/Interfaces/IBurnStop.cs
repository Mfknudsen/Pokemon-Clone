using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IBurnStop
    {
        public bool CanStopBurn(Pokemon pokemon);
    }
}
