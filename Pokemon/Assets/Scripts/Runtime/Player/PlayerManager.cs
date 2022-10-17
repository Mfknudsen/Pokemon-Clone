#region Packages

using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Common;
using Runtime.Files;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Runtime.Systems.PersistantRunner;
using Runtime.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.Player
{
    [CreateAssetMenu(menuName = "Manager/Player")]
    public sealed class PlayerManager : Manager, IFrameStart
    {
        #region Values

        [FoldoutGroup("References")] [SerializeField]
        private Team team;

        [FoldoutGroup("References")] [SerializeField]
        private Controller controller;

        [FoldoutGroup("References")] [SerializeField]
        private NavMeshAgent agent;

        [FoldoutGroup("References")] [SerializeField]
        private BattleMember battleMember;

        [FoldoutGroup("References")] [SerializeField]
        private PlayerInteractions playerInteractions;

        [FoldoutGroup("References")] [SerializeField]
        private Controller moveController;

        [FoldoutGroup("References")] [SerializeField]
        private CinemachineFreeLook overworldCameraRig;

        [FoldoutGroup("Character Sheet"), HideLabel] [SerializeField]
        private CharacterSheet characterSheet;

        [FoldoutGroup("Variables")] [SerializeField, Required]
        private Vec2Variable moveDirection, rotationDirection;

        [FoldoutGroup("Variables")] [SerializeField, Required]
        private BoolVariable running;

        [BoxGroup("Variables/Camera")] [SerializeField, Required]
        private CinemachineBrainVariable cameraBrain;

        [BoxGroup("Variables/Camera")] [SerializeField, Required]
        private CinemachineFreeLookVariable defaultOverworldRig;

        private GameObject overworldGameObject;

        private const string FileName = "PlayerData";

        #endregion

        #region Build In State

        // ReSharper disable Unity.PerformanceAnalysis
        public void FrameStart()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.AddListener(this.OnMoveAxisChange);
            inputManager.turnAxisInputEvent.AddListener(this.OnTurnAxisChange);
            inputManager.runInputEvent.AddListener(this.OnRunChange);

            this.overworldGameObject = GameObject.Find("Player");

            GameObject overworld = this.overworldGameObject.GetChildByName("Overworld");

            this.overworldCameraRig = overworld.GetFirstComponentByRoot<CinemachineFreeLook>();

            this.defaultOverworldRig.value = this.overworldCameraRig;

            this.moveController = overworld.GetComponent<Controller>();

            this.playerInteractions = overworld.GetComponent<PlayerInteractions>();

            this.cameraBrain.value = this.overworldGameObject.GetComponentInChildren<CinemachineBrain>();

            this.characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(FileName));
            this.overworldCameraRig.enabled = false;
            this.moveController.Setup();

            Debug.Log("d: " + this.overworldGameObject.GetFirstComponentByRoot<BattleMember>());
            this.battleMember = this.overworldGameObject.GetFirstComponentByRoot<BattleMember>();

            this.playerInteractions.StartCoroutine(this.playerInteractions.Setup());
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.RemoveListener(this.OnMoveAxisChange);
            inputManager.turnAxisInputEvent.RemoveListener(this.OnTurnAxisChange);
            inputManager.runInputEvent.RemoveListener(this.OnRunChange);
        }

        #endregion

        #region Getters

        public CharacterSheet GetCharacterSheet() => this.characterSheet;

        public string[] GetPronouns() => new[]
            { this.characterSheet.pronoun1, this.characterSheet.pronoun2, this.characterSheet.pronoun3 };

        public Team GetTeam() => this.team;

        public BattleMember GetBattleMember() => this.battleMember;

        public PlayerInteractions GetInteractions() => this.playerInteractions;

        public NavMeshAgent GetAgent() => this.agent;

        public CinemachineFreeLook GetOverworldCameraRig() => this.overworldCameraRig;

        public Controller GetController() => this.controller;

        #endregion

        #region In

        public void EnablePlayerControl() => this.controller.Enable();

        public void DisablePlayerControl() => this.controller.Disable();

        public void DisableOverworld() => this.overworldGameObject.SetActive(false);

        public void EnableOverworld() => this.overworldGameObject.SetActive(true);

        public void PlayAnimationClip(AnimationClip clip)
        {
        }

        #endregion

        #region Internal

        #region Input

        private void OnMoveAxisChange(Vector2 set) => this.moveDirection.value = set;

        private void OnTurnAxisChange(Vector2 set) => this.rotationDirection.value = set;

        private void OnRunChange(bool set) => this.running.value = set;

        #endregion

        #endregion
    }
}