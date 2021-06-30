using System.Collections;
using System.Linq;

namespace Mfknudsen.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            BattleStarter starter = master.GetStarter();

            bool playerVictory = false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (BattleMember battleMember in master.GetMembers().Where(m => m.GetTeamNumber() != 1))
            {
                if (!battleMember.GetTeam().HasMorePokemon()) continue;

                playerVictory = true;
                
                break;
            }

            yield return 0;

            starter.EndBattle(playerVictory);
        }
    }
}