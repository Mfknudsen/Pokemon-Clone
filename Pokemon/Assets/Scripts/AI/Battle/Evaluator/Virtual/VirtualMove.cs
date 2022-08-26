#region Packages

using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Battle.Evaluator.Virtual
{
    public class VirtualMove
    {
        public readonly float value;
        public readonly VirtualBattle virtualBattle;
        public readonly BattleAction rootAction;

        public VirtualMove(BattleAction rootAction, float preValue,
            BattleAction action, Pokemon user, Pokemon target,
            VirtualBattle virtualBattle, PersonalitySetting personalitySetting)
        {
            this.rootAction = rootAction;
            this.virtualBattle = new VirtualBattle(virtualBattle);
            value = preValue + action.Evaluate(user, target, this.virtualBattle, personalitySetting);
        }
    }
}