#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Communication;
using UnityEngine;
using Mfknudsen.Player;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.UI;
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

        public delegate void OnBattleEnd(bool playerWon);

        public OnBattleEnd onBattleEnd;

        #endregion

        [SerializeField] private string battleSceneName = "";

        [SerializeField] private int playerSpotCount = 1;
        [SerializeField] private BattleMember[] allies;
        [SerializeField] private BattleMember[] enemies;
        [SerializeField] private Chat onStartChat;

        [SerializeField] private Transition transition;

        private bool ready = true, playerWon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!(playerSpotCount >= 1 && playerSpotCount <= 3))
                Debug.LogError("Player Spot Count Must Be Between 1 and 3");
        }
#endif

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

        public bool GetPlayerWon()
        {
            return playerWon;
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
            if (!ready) return;

            ready = false;

            WorldManager manager = WorldManager.instance;

            transition.onHide = () =>
            {
                PlayerManager.instance.DisableOverworld();
                UIBook.instance.gameObject.SetActive(false);
                UIManager.instance.SwitchUI(UISelection.Battle);
            };

            manager.SetTransition(transition);
            manager.LoadBattleScene(battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            StartCoroutine(WaitForResponse());
        }

        public void EndBattle(bool playerVictory)
        {
            transition.onHide = () =>
            {
                UIBook.instance.gameObject.SetActive(true);
                UIManager.instance.SwitchUI(UISelection.Start);
                UIBook.instance.Effect(BookTurn.Open);
            };

            WorldManager manager = WorldManager.instance;
            manager.SetTransition(transition);
            manager.UnloadCurrentBattleScene();

            playerWon = playerVictory;

            onBattleEnd?.Invoke(playerVictory);
        }

        private IEnumerator WaitForResponse()
        {
            while (BattleManager.instance == null)
                yield return null;

            List<BattleMember> result = new List<BattleMember> { PlayerManager.instance.GetComponent<BattleMember>() };
            result[0].SetTeamNumber(true);
            foreach (BattleMember m in allies
                         .Where(m =>
                             m != null))
            {
                m.SetTeamNumber(true);
                if (!result.Contains(m))
                    result.Add(m);
            }

            foreach (BattleMember m in enemies
                         .Where(m =>
                             m != null))
            {
                m.SetTeamNumber(false);
                if (!result.Contains(m))
                    result.Add(m);
            }

            onBattleStart?.Invoke();

            BattleManager.instance.StartBattle(this);

            Chat toSend = Instantiate(onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", enemies[0].GetName());
            ChatManager.instance.Add(new[] { toSend });
        }

        #endregion
    }
}