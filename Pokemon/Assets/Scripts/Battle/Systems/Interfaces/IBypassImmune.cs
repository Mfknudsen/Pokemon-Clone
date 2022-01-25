using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    public interface IBypassImmune
    {
        public bool CanEffect(TypeName attackType, TypeName defendsType);
    }
}
