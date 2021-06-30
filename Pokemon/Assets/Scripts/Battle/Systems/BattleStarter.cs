#region SDK

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mfknudsen.Comunication;
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
        [SerializeField] private BattleMember[] allies = new BattleMember[0];
        [SerializeField] private BattleMember[] enemies = new BattleMember[0];
        [SerializeField] private Chat onStartChat = null;

        [Header("Before/After:")] [SerializeField]
        private Dictionary<GameObject, bool> checkList = new Dictionary<GameObject, bool>();

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

        #endregion

        #region In

        public void StartBattleNow()
        {
            GameObject obj = GameObject.FindGameObjectWithTag("UI Canvas");
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject o = obj.transform.GetChild(i).gameObject;
                checkList.Add(o, o.activeSelf);
                o.SetActive(false);
            }

            WorldMaster.instance.LoadBattleScene(battleSceneName);

            StartCoroutine(WaitForRespons());
        }

        public void EndBattle(bool playerVictory)
        {
            WorldMaster.instance.UnloadCurrentBattleScene();

            foreach (GameObject key in checkList.Keys)
                key.SetActive(checkList[key]);

            if (playerVictory)
                Debug.Log("You Win!");
            else
                Debug.Log("You Lose!");
        }

        IEnumerator WaitForRespons()
        {
            while (BattleMaster.instance == null)
                yield return null;

            List<BattleMember> result = new List<BattleMember>();
            result.Add(MasterPlayer.instance.GetComponent<BattleMember>());
            result[0].SetTeamNumber(0);
            foreach (BattleMember m in allies)
            {
                if (m != null)
                {
                    m.SetTeamNumber(0);
                    if (!result.Contains(m))
                        result.Add(m);
                }
            }

            foreach (BattleMember m in enemies)
            {
                if (m != null)
                {
                    m.SetTeamNumber(1);
                    if (!result.Contains(m))
                        result.Add(m);
                }
            }

            BattleMaster.instance.StartBattle(this, result.ToArray());

            Chat toSend = Instantiate(onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", enemies[0].GetName());
            ChatMaster.instance.Add(new Chat[] {toSend});
        }

        #endregion
    }
}