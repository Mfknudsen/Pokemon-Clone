using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IBypassStatus
    {
        public bool CanEffect(Type type);
    }
}
