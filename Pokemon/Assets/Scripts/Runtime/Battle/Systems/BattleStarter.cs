#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Battle;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Player.Camera;
using Runtime.Systems.UI;
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

        [SerializeField, Required] private TileManager tileManager;
        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;
        [SerializeField, Required] private CameraManager cameraManager;
        [SerializeField, Required] private ChatManager chatManager;
        [SerializeField, Required] private WorldManager worldManager;


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
            if (playerSpotCount is not (>= 1 and <= 3))
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
            List<BattleMember> result = new() { playerManager.GetBattleMember() };
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
                tileManager.HideTiles();
                playerManager.DisableOverworld();
                UIBook.instance.gameObject.SetActive(false);
                uiManager.SwitchUI(UISelection.Battle);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            };

            onBattleEnd += delegate { StartCoroutine(gameObject.GetComponent<UnitBattleBase>()?.AfterBattle()); };

            worldManager.SetTransition(transition);
            worldManager.LoadBattleScene(battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            StartCoroutine(WaitForResponse());
        }

        private IEnumerator WaitForResponse()
        {
            yield return null;

            yield return new WaitWhile(() => !BattleManager.instance);

            List<BattleMember> result = new() { playerManager.GetBattleMember() };
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
            chatManager.Add(new[] { toSend });
        }

        public void EndBattle(bool playerVictory)
        {
            transition.onHide = () =>
            {
                tileManager.ShowTiles();
                playerManager.EnableOverworld();
                UIBook.instance.gameObject.SetActive(true);
                uiManager.SwitchUI(UISelection.Overworld);
                cameraManager.SetCurrentRigToDefault();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                //UIBook.instance.gameObject.SetActive(true);
                //UIManager.instance.SwitchUI(UISelection.Start);
                //UIBook.instance.Effect(BookTurn.Open);
                transform.parent = overworldParent;
            };

            worldManager.SetTransition(transition);
            worldManager.UnloadCurrentBattleScene();

            playerWon = playerVictory;

            onBattleEnd?.Invoke(playerVictory);
        }

        #endregion
    }
}