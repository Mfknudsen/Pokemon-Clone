#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Battle;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Player.Camera;
using Runtime.UI;
using Runtime.UI_Book;
using Runtime.UI.SceneTransitions.Transitions;
using Runtime.World;
using Runtime.World.Overworld.Tiles;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems
{
    public class BattleStarter : MonoBehaviour
    {
        #region Values

        #region Delegates

        public delegate void OnBattleEnd(bool playerWon);

        public OnBattleEnd onBattleEnd;

        #endregion

        [SerializeField] private string battleSceneName = "";
        [SerializeField] private int playerSpotCount = 1;

        [FoldoutGroup("BattleMembers")] [SerializeField]
        private BattleMember[] allies, enemies;

        [SerializeField] private Chat onStartChat;
        [SerializeField] private Transition transition;

        private bool ready = true, playerWon;

        private Transform overworldParent;

        private void OnValidate()
        {
            if (!(playerSpotCount >= 1 && playerSpotCount <= 3))
                Debug.LogError("Player Spot Count Must Be Between 1 and 3");
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
            List<BattleMember> result = new() { PlayerManager.instance.GetBattleMember() };
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

            Transform t = transform;
            overworldParent = t.parent;
            t.parent = null;

            transition.onHide = () =>
            {
                TileManager.instance.HideTiles();
                PlayerManager.instance.DisableOverworld();
                UIBook.instance.gameObject.SetActive(false);
                UIManager.instance.SwitchUI(UISelection.Battle);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            };

            onBattleEnd += delegate { StartCoroutine(gameObject.GetComponent<NpcBattleBase>()?.AfterBattle()); };

            WorldManager manager = WorldManager.instance;
            manager.SetTransition(transition);
            manager.LoadBattleScene(battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            StartCoroutine(WaitForResponse());
        }

        private IEnumerator WaitForResponse()
        {
            yield return null;

            yield return new WaitWhile(() => !BattleManager.instance);

            List<BattleMember> result = new() { PlayerManager.instance.GetBattleMember() };
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

            BattleManager.instance.StartBattle(this);

            Chat toSend = Instantiate(onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", enemies[0].GetName());
            ChatManager.instance.Add(new[] { toSend });
        }

        public void EndBattle(bool playerVictory)
        {
            transition.onHide = () =>
            {
                TileManager.instance.ShowTiles();
                PlayerManager.instance.EnableOverworld();
                UIBook.instance.gameObject.SetActive(true);
                UIManager.instance.SwitchUI(UISelection.Overworld);
                CameraManager.instance.SetCurrentRigToDefault();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                //UIBook.instance.gameObject.SetActive(true);
                //UIManager.instance.SwitchUI(UISelection.Start);
                //UIBook.instance.Effect(BookTurn.Open);
                transform.parent = overworldParent;
            };

            WorldManager manager = WorldManager.instance;
            manager.SetTransition(transition);
            manager.UnloadCurrentBattleScene();

            playerWon = playerVictory;

            onBattleEnd?.Invoke(playerVictory);
        }

        #endregion
    }
}