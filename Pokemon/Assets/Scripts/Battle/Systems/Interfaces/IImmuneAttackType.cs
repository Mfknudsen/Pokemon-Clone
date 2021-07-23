using Mfknudsen.Pokémon;

namespace Mfknudsen.Battle.Systems.Interfaces
{
    // ReSharper disable once IdentifierTypo
    public interface IImmuneAttackType
    {
        public bool MatchType(TypeName type);
    }
}
