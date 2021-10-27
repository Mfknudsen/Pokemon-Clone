#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Communication;
using UnityEngine;
using Mfknudsen.Player;
using Mfknudsen.World;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class BattleStarter : MonoBehaviour
    {
        #region Values

        [Header("Battle Reference:")] [SerializeField]
        private string battleSceneName = "";

        [SerializeField] private int playerSpotCount = 1;
        [SerializeField] private BattleMember[] allies;
        [SerializeField] private BattleMember[] enemies;
        [SerializeField] private Chat onStartChat;

        [Header("Before/After:")] private Dictionary<GameObject, bool> checkList = new Dictionary<GameObject, bool>();

        private void OnValidate()
        {
            playerSpotCount = Mathf.Clamp(playerSpotCount, 1, 3);
        }

        #endregion

        #region Getters

        public int GetPlayerSpotCount()
        {
            return playerSpotCount;
        }

        public int GetAllySpotCount()
        {
            return allies.Sum(battleMember => battleMember.GetSpotsToOwn());
        }

        public int GetEnemiesSpotCount()
        {
            return enemies.Sum(battleMember => battleMember.GetSpotsToOwn());
        }

        public BattleMember[] GetAllies()
        {
            return allies;
        }

        public BattleMember[] GetEnemies()
        {
            return enemies;
        }

        public List<BattleMember> GetAllBattleMembers()
        {
            List<BattleMember> result = new List<BattleMember> { PlayerManager.instance.GetBattleMember() };
            result.AddRange(allies);
            result.AddRange(enemies);
            return result;
        }

        #endregion

        #region In

        public void StartBattleNow()
        {
            WorldManager.instance.LoadBattleScene(battleSceneName);

            StartCoroutine(WaitForResponse());
        }

        public void EndBattle(bool playerVictory)
        {
            WorldManager.instance.UnloadCurrentBattleScene();

            foreach (GameObject key in checkList.Keys)
                key.SetActive(checkList[key]);

            Debug.Log(playerVictory ? "You Win!" : "You Lose!");
        }

        private IEnumerator WaitForResponse()
        {
            while (BattleManager.instance == null)
                yield return null;

            List<BattleMember> result = new List<BattleMember> { PlayerManager.instance.GetComponent<BattleMember>() };
            result[0].SetTeamNumber(true);
            foreach (BattleMember m in allies)
            {
                if (m == null) continue;

                m.SetTeamNumber(true);
                if (!result.Contains(m))
                    result.Add(m);
            }

            foreach (BattleMember m in enemies)
            {
                if (m == null) continue;

                m.SetTeamNumber(false);
                if (!result.Contains(m))
                    result.Add(m);
            }

            BattleManager.instance.StartBattle(this);

            Chat toSend = Instantiate(onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", enemies[0].GetName());
            ChatManager.instance.Add(new[] { toSend });
        }

        #endregion
    }
}