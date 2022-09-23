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
            if (this.playerSpotCount is not (>= 1 and <= 3))
                Debug.LogError("Player Spot Count Must Be Between 1 and 3");
        }

        #endregion

        #region Getters

        public int GetPlayerSpotCount()
        {
            return this.playerSpotCount;
        }

        public int GetAllySpotCount()
        {
            return this.allies.Sum(battleMember => battleMember.GetSpotsToOwn());
        }

        public int GetEnemiesSpotCount()
        {
            return this.enemies.Sum(battleMember => battleMember.GetSpotsToOwn());
        }

        public bool GetPlayerWon()
        {
            return this.playerWon;
        }

        public BattleMember[] GetAllies()
        {
            return this.allies;
        }

        public BattleMember[] GetEnemies()
        {
            return this.enemies;
        }

        public List<BattleMember> GetAllBattleMembers()
        {
            List<BattleMember> result = new() { this.playerManager.GetBattleMember() };
            result.AddRange(this.allies);
            result.AddRange(this.enemies);
            return result;
        }

        #endregion

        #region In

        public void StartBattleNow()
        {
            if (!this.ready) return;

            this.ready = false;

            Transform t = transform;
            this.overworldParent = t.parent;
            t.parent = null;

            this.transition.onHide = () =>
            {
                this.tileManager.HideTiles();
                this.playerManager.DisableOverworld();
                UIBook.instance.gameObject.SetActive(false);
                this.uiManager.SwitchUI(UISelection.Battle);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            };

            this.onBattleEnd += delegate { StartCoroutine(gameObject.GetComponent<UnitBattleBase>()?.AfterBattle()); };

            this.worldManager.SetTransition(this.transition);
            this.worldManager.LoadBattleScene(this.battleSceneName);

            //Wait for the Battle Scene to load and apply settings from Battle Starter
            StartCoroutine(WaitForResponse());
        }

        private IEnumerator WaitForResponse()
        {
            yield return null;

            yield return new WaitWhile(() => !BattleManager.instance);

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

            BattleManager.instance.StartBattle(this);

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
                UIBook.instance.gameObject.SetActive(true);
                this.uiManager.SwitchUI(UISelection.Overworld);
                this.cameraManager.SetCurrentRigToDefault();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                //UIBook.instance.gameObject.SetActive(true);
                //UIManager.instance.SwitchUI(UISelection.Start);
                //UIBook.instance.Effect(BookTurn.Open);
                transform.parent = this.overworldParent;
            };

            this.worldManager.SetTransition(this.transition);
            this.worldManager.UnloadCurrentBattleScene();

            this.playerWon = playerVictory;

            this.onBattleEnd?.Invoke(playerVictory);
        }

        #endregion
    }
}