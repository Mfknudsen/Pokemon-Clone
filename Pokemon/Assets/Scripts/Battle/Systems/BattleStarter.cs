#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Communication;
using UnityEngine;
using Mfknudsen.Player;
using Mfknudsen.UI.Scene_Transitions.Transitions;
using Mfknudsen.World;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class BattleStarter : MonoBehaviour
    {
        #region Values

        #region Delegates

        public delegate void OnBattleStart();

        public OnBattleStart onBattleStart;

        public delegate void OnBattleEnd();

        public OnBattleEnd onBattleEnd;

        #endregion

        [SerializeField] private string battleSceneName = "";

        [SerializeField] private int playerSpotCount = 1;
        [SerializeField] private BattleMember[] allies;
        [SerializeField] private BattleMember[] enemies;
        [SerializeField] private Chat onStartChat;

        [SerializeField] private Transition transition;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!(playerSpotCount >= 1 && playerSpotCount <= 3))
                Debug.LogError("Player Spot Count Must Be Between 1 and 3");
#endif
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
            List<BattleMember> result = new List<BattleMember> {PlayerManager.instance.GetBattleMember()};
            result.AddRange(allies);
            result.AddRange(enemies);
            return result;
        }

        #endregion

        #region In

        public void StartBattleNow()
        {
            WorldManager manager = WorldManager.instance;

            manager.SetTransition(transition);
            manager.LoadBattleScene(battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            StartCoroutine(WaitForResponse());
        }

        public void EndBattle(bool playerVictory)
        {
            WorldManager.instance.UnloadCurrentBattleScene();

            Debug.Log(playerVictory ? "You Win!" : "You Lose!");

            onBattleEnd?.Invoke();
        }

        private IEnumerator WaitForResponse()
        {
            while (BattleManager.instance == null)
                yield return null;

            List<BattleMember> result = new List<BattleMember> {PlayerManager.instance.GetComponent<BattleMember>()};
            result[0].SetTeamNumber(true);
            foreach (BattleMember m in allies.Where(m => m != null))
            {
                m.SetTeamNumber(true);
                if (!result.Contains(m))
                    result.Add(m);
            }

            foreach (BattleMember m in enemies.Where(m => m != null))
            {
                m.SetTeamNumber(false);
                if (!result.Contains(m))
                    result.Add(m);
            }

            onBattleStart.Invoke();

            BattleManager.instance.StartBattle(this);

            Chat toSend = Instantiate(onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", enemies[0].GetName());
            ChatManager.instance.Add(new[] {toSend});
        }

        #endregion
    }
}