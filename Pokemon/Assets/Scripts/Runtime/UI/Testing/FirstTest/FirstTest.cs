#region Packages

using System.Collections.Generic;
using Runtime.Battle.Systems;
using Runtime.Player;
using Runtime.Pokémon;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.UI.Testing.FirstTest
{
    public class FirstTest : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private PlayerManager playerManager;
        
        #region Wild

        [FoldoutGroup("Wild Encounter Settings")] [SerializeField]
        private GameObject wildBasePrefab;

        [FoldoutGroup("Wild Encounter Settings/poke")] [SerializeField]
        private List<Pokemon> possibleEncounters = new();

        [FoldoutGroup("trainer Encounter")] [SerializeField]
        private GameObject trainerBasePrefab;

        #endregion

        #endregion

        #region In

        public GameObject SpawnWildEncounter()
        {
            GameObject obj = Instantiate(wildBasePrefab);
            BattleMember member = obj.GetComponent<BattleMember>();
            BattleStarter starter = obj.GetComponent<BattleStarter>();

            Pokemon pokemon = possibleEncounters[Random.Range(0, possibleEncounters.Count)];
            member.GetTeam().AddNewPokemonToTeam(pokemon);

            starter.onBattleEnd += _ => { Destroy(obj); };

            return obj;
        }

        public GameObject SpawnTrainerEncounter()
        {
            GameObject obj = Instantiate(trainerBasePrefab);
            BattleMember member = obj.GetComponent<BattleMember>();
            BattleStarter starter = obj.GetComponent<BattleStarter>();

            for (int i = 0; i < playerManager.GetTeam().GetTeamCount(); i++)
            {
                Pokemon pokemon = possibleEncounters[Random.Range(0, possibleEncounters.Count)];
                member.GetTeam().AddNewPokemonToTeam(pokemon);
            }

            starter.onBattleEnd += _ => { Destroy(obj); };

            return obj;
        }

        #endregion
    }
}