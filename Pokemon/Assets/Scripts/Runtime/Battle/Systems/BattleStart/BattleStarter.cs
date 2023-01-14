#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Runtime.AI;
using Runtime.Battle.Systems.Initializer;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Player.Camera;
using Runtime.Systems;
using Runtime.Systems.UI;
using Runtime.World.Overworld.Interactions;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.BattleStart
{
    public abstract class BattleStarter : MonoBehaviour, IInteractable
    {
        #region Values

        #region Delegates

        public delegate void OnBattleEnd(bool playerWon);

        public OnBattleEnd onBattleEnd;

        #endregion

        [SerializeField, AssetSelector(Paths = "Assets/Prefabs/Battle/Systems"), Required]
        private BattleSystem battleSystem;

        [SerializeField, AssetSelector(Paths = "Assets/Prefabs/Battle/Initializers"), Required]
        private BattleInitializer battleInitializer;

        [SerializeField, Required] protected UnitManager unitManager;
        [SerializeField, Required] protected PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;
        [SerializeField, Required] private CameraManager cameraManager;
        [SerializeField, Required] protected ChatManager chatManager;
        [SerializeField, Required] private OperationManager operationManager;

        [SerializeField, Min(1), MaxValue(3)] private int playerSpotCount = 1;

        [FoldoutGroup("BattleMembers")] [SerializeField]
        protected BattleMember[] allies, enemies;

        [SerializeField, FoldoutGroup("Intro and Outro"), Required]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField, FoldoutGroup("Intro and Outro"), Required]
        private Chat onStartChat, onEndChat;

        [SerializeField, FoldoutGroup("Intro and Outro")]
        private CameraEvent introEvent, outroEvent;

        private bool ready = true, playerWon;

        private BattleSystem instantiatedBattleSystem;
        private BattleInitializer instantiatedBattleInitializer;

        private Vector3 playerOldPosition;
        private Quaternion playerOldRotation;

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

        public BattleInitializer GetBattleInitializer => this.battleInitializer;

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetInitializer(BattleInitializer set) =>
            this.battleInitializer = set;
#endif

        #endregion

        #region In

        public abstract void InteractTrigger();

        public void TriggerBattle()
        {
            if (!this.ready) return;

            this.unitManager.PauseAllUnits();
            this.playerManager.DisablePlayerControl();

            this.playerOldPosition = this.playerManager.GetController().transform.position;
            this.playerOldRotation = this.playerManager.GetController().transform.rotation;

            this.ready = false;

            this.StartCoroutine(this.SetupBattle());
        }

        private IEnumerator SetupBattle()
        {
            Chat instantiatedChat = this.onStartChat.GetChatInstantiated();
            instantiatedChat.AddToOverride("<TRAINER_NAME>", this.enemies[0].GetName());
            for (int i = 0; i < this.enemies.Length; i++)
                instantiatedChat.AddToOverride($"<TRAINER_NAME_{i}>", this.enemies[i].GetName());

            this.chatManager.Add(instantiatedChat);

            OperationsContainer container = new(this.introEvent);
            this.operationManager.AddOperationsContainer(container);

            Transform starterTransform = this.transform;
            Quaternion starterRotation = starterTransform.rotation;
            
            this.instantiatedBattleInitializer = Instantiate(this.battleInitializer, starterTransform);
            Transform initTransform = this.instantiatedBattleInitializer.transform;
            initTransform.localPosition = Vector3.zero;
            initTransform.rotation = starterRotation;
            
            this.instantiatedBattleSystem = Instantiate(this.battleSystem, starterTransform);
            Transform systemTransform = this.instantiatedBattleSystem.transform;
            systemTransform.localPosition = Vector3.zero;
            systemTransform.rotation = starterRotation;

            this.instantiatedBattleInitializer.FindSetupBattleZone();

            yield return new WaitUntil(() =>
                this.chatManager.GetIsClear() &&
                this.instantiatedBattleInitializer.HasFoundBattleZone &&
                this.introEvent.IsOperationDone);

            Transform playerTransform = this.playerManager.GetController().transform;
            playerTransform.position = this.battleInitializer.GetPlayerCharacterPosition().position;
            playerTransform.rotation = this.battleInitializer.GetPlayerCharacterPosition().rotation;

            container = new OperationsContainer(this.cameraManager.ReturnToDefaultOverworldEvent());
            this.operationManager.AddOperationsContainer(container);

            this.uiManager.SwitchUI(UISelection.Battle);

            yield return new WaitUntil(() => this.operationManager.GetDone());

            this.chatManager.ShowTextField(false);

            this.instantiatedBattleSystem.StartBattle(this);
        }

        public void EndBattle(bool playerVictory)
        {
            Destroy(this.instantiatedBattleSystem.gameObject);
            Destroy(this.instantiatedBattleInitializer.gameObject);

            Chat instantiatedChat = this.onEndChat.GetChatInstantiated();
            instantiatedChat.AddToOverride("<TRAINER_NAME>", this.enemies[0].GetName());
            for (int i = 0; i < this.enemies.Length; i++)
                instantiatedChat.AddToOverride($"<TRAINER_NAME_{i}>", this.enemies[i].GetName());

            Transform t = this.playerManager.GetController().transform;
            t.position = this.playerOldPosition;
            t.rotation = this.playerOldRotation;

            this.playerManager.EnablePlayerControl();

            this.uiManager.SwitchUI(UISelection.Overworld);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            this.playerWon = playerVictory;

            this.onBattleEnd?.Invoke(playerVictory);
        }

        #endregion
    }
}