#region Packages

using System.Collections;
using System.Linq;
using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Files;
using Runtime.Items;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Runtime.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.Player
{
    [RequireComponent(
        typeof(BattleMember),
        typeof(Team),
        typeof(Inventory))]
    public class PlayerManager : Manager
    {
        #region Values

        public static PlayerManager instance;

        [FoldoutGroup("References")] [SerializeField, Required]
        private Team team;

        [FoldoutGroup("References")] [SerializeField, Required]
        private Controller controller;

        [FoldoutGroup("References")] [SerializeField, Required]
        private NavMeshAgent agent;

        [FoldoutGroup("References")] [SerializeField, Required]
        private BattleMember battleMember;

        [FoldoutGroup("References")] [SerializeField, Required]
        private PlayerInteractions playerInteractions;

        [FoldoutGroup("References")] [SerializeField, Required]
        private Controller moveController;

        [FoldoutGroup("References")] [SerializeField, Required]
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

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.AddListener(OnMoveAxisChange);
            inputManager.turnAxisInputEvent.AddListener(OnTurnAxisChange);
            inputManager.runInputEvent.AddListener(OnRunChange);

            this.defaultOverworldRig.value = GetComponentsInChildren<CinemachineFreeLook>()
                .First(c => c.name.Equals("Player Third Person Rig"));

            this.cameraBrain.value = GetComponentInChildren<CinemachineBrain>();
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.RemoveListener(OnMoveAxisChange);
            inputManager.turnAxisInputEvent.RemoveListener(OnTurnAxisChange);
            inputManager.runInputEvent.RemoveListener(OnRunChange);
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

        public override IEnumerator Setup()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);

            this.characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(FileName));
            this.overworldCameraRig.enabled = false;
            this.overworldGameObject = this.controller.gameObject;
            this.moveController.Setup();

            StartCoroutine(this.playerInteractions.Setup());

            yield break;
        }

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

        private void OnMoveAxisChange(Vector2 vec) => this.moveDirection.value = vec;

        private void OnTurnAxisChange(Vector2 vec) => this.rotationDirection.value = vec;

        private void OnRunChange(bool set) => this.running.value = set;

        #endregion

        #endregion
    }
}