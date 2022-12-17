#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems;
using Runtime.Systems.UI;
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

        [SerializeField, Required] private BattleSystem battleSystem;

        [SerializeField, Required] private UnitManager unitManager;
        [SerializeField, Required] private TileManager tileManager;
        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;
        [SerializeField, Required] private CameraManager cameraManager;
        [SerializeField, Required] private ChatManager chatManager;
        [SerializeField, Required] private WorldManager worldManager;

        [SerializeField, Min(1), MaxValue(3)] private int playerSpotCount = 1;

        [FoldoutGroup("BattleMembers")] [SerializeField]
        private BattleMember[] allies, enemies;

        [SerializeField] private Chat onStartChat, onEndChat;
        [SerializeField] private Transition transition;

        private bool ready = true, playerWon;
        
        private BattleSystem instantiatedBattleSystem;

        #endregion

        #region Getters

        public bool GetIsBattleReady() => this.ready;

        public int GetPlayerSpotCount() => this.playerSpotCount;

        public int GetAllySpotCount() => this.allies.Sum(battleMember => battleMember.GetSpotsToOwn());

        public int GetEnemiesSpotCount() => this.enemies.Sum(battleMember => battleMember.GetSpotsToOwn());

        public bool GetPlayerWon() => this.playerWon;

        public BattleMember[] GetAllies() => this.allies;

        public BattleMember[] GetEnemies() => this.enemies;

        public List<BattleMember> GetAllBattleMembers()
        {
            List<BattleMember> result = new() { this.playerManager.GetBattleMember() };
            result.AddRange(this.allies);
            result.AddRange(this.enemies);
            return result;
        }

        #endregion

        #region In

        public void TriggerBattle()
        {
            if (!this.ready) return;

            this.unitManager.PauseAllUnits();
            this.playerManager.DisableOverworld();

            this.ready = false;

            Debug.Log("Setting up battle");
            
            this.StartCoroutine(this.SetupBattle());

            /*
            Transform t = this.transform;
            this.overworldParent = t.parent;
            t.parent = null;

            this.transition.onHide = () =>
            {
                this.tileManager.HideTiles();
                this.playerManager.DisableOverworld();
                this.uiManager.UIBook.gameObject.SetActive(false);
                this.uiManager.SwitchUI(UISelection.Battle);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            };

            this.onBattleEnd += delegate
            {
                this.StartCoroutine(this.gameObject.GetComponent<UnitBattleBase>()?.AfterBattle());
                this.unitManager.ResumeAllUnits();
            };

            this.worldManager.SetTransition(this.transition);
            this.worldManager.LoadBattleScene(this.battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            this.StartCoroutine(this.WaitForResponse());
            */
        }

        private IEnumerator SetupBattle()
        {
            this.instantiatedBattleSystem = Instantiate(this.battleSystem);

            Chat instantiatedChat = Instantiate(this.onStartChat);
            this.chatManager.Add(instantiatedChat);

            yield return null;

            yield return new WaitUntil(() => instantiatedChat.GetDone());
        }

        private IEnumerator WaitForResponse()
        {
            yield return null;

            yield return new WaitWhile(() => !BattleSystem.instance);

            List<BattleMember> result = new() { this.playerManager.GetBattleMember() };
            result[0].SetTeamNumber(true);
            foreach (BattleMember m in this.allies
                         .Where(m =>
                             m != null))
            {
                m.SetTeamNumber(true);
                if (!result.Contains(m))
                    result.Add(m);
            }

            foreach (BattleMember m in this.enemies
                         .Where(m =>
                             m != null))
            {
                m.SetTeamNumber(false);
                if (!result.Contains(m))
                    result.Add(m);
            }

            BattleSystem.instance.StartBattle(this);

            Chat toSend = Instantiate(this.onStartChat);
            toSend.AddToOverride("<TRAINER_NAME>", this.enemies[0].GetName());
            this.chatManager.Add(new[] { toSend });
        }

        public void EndBattle(bool playerVictory)
        {
            this.transition.onHide = () =>
            {
                this.tileManager.ShowTiles();
                this.playerManager.EnableOverworld();
                this.uiManager.UIBook.gameObject.SetActive(true);
                this.uiManager.SwitchUI(UISelection.Overworld);
                this.cameraManager.SetCurrentRigToDefault();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                //UIBook.instance.gameObject.SetActive(true);
                //UIManager.instance.SwitchUI(UISelection.Start);
                //UIBook.instance.Effect(BookTurn.Open);
            };

            this.worldManager.SetTransition(this.transition);
            this.worldManager.UnloadCurrentBattleScene();

            this.playerWon = playerVictory;

            this.onBattleEnd?.Invoke(playerVictory);
        }

        #endregion
    }
}