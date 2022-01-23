#region Packages

using System.Collections.Generic;
using JetBrains.Annotations;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Testing.FirstTest
{
    public class FirstTest : MonoBehaviour
    {
        #region Values

        #region Wild

        [SerializeField] [FoldoutGroup("Wild Encounter Settings")]
        private GameObject wildBasePrefab;

        [SerializeField] [FoldoutGroup("Wild Encounter Settings/poke")]
        private List<Pokemon> possibleEncounters = new List<Pokemon>();

        #endregion

        #endregion

        #region In

        #region Buttons

        [UsedImplicitly]
        public void SpawnWildEncounter()
        {
            GameObject obj = Instantiate(wildBasePrefab);
            BattleMember member = obj.GetComponent<BattleMember>();
            BattleStarter starter = obj.GetComponent<BattleStarter>();
            
            Pokemon pokemon = possibleEncounters[Random.Range(0, possibleEncounters.Count)];
            member.GetTeam().AddNewPokemonToTeam(pokemon);

            starter.onBattleEnd += () => { Destroy(obj); };

            starter.StartBattleNow();
        }

        [UsedImplicitly]
        public void SpawnTrainerEncounter()
        {
            GameObject obj = Instantiate(wildBasePrefab);
            BattleMember member = obj.GetComponent<BattleMember>();
            BattleStarter starter = obj.GetComponent<BattleStarter>();
            
            for (int i = 0; i < PlayerManager.instance.GetTeam().GetTeamCount(); i++)
            {
                Pokemon pokemon = possibleEncounters[Random.Range(0, possibleEncounters.Count)];
                member.GetTeam().AddNewPokemonToTeam(pokemon);
                
            }
            starter.onBattleEnd += () => { Destroy(obj); };

            starter.StartBattleNow();
        }

        #endregion

        #endregion
    }
}