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

        [FoldoutGroup("Variables")] [SerializeField]
        private Vec2Variable moveDirection, rotationDirection;

        [FoldoutGroup("Variables")] [SerializeField]
        private BoolVariable running;

        [BoxGroup("Variables/Camera")] [SerializeField]
        private CinemachineBrainVariable cameraBrain;

        [BoxGroup("Variables/Camera")] [SerializeField]
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

            defaultOverworldRig.value = GetComponentsInChildren<CinemachineFreeLook>()
                .First(c => c.name.Equals("Player Third Person Rig"));

            if (UnityEngine.Camera.main != null)
                cameraBrain.value = UnityEngine.Camera.main.GetComponent<CinemachineBrain>();
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

        public CharacterSheet GetCharacterSheet() => characterSheet;

        public string[] GetPronouns() => new[] { characterSheet.pronoun1, characterSheet.pronoun2, characterSheet.pronoun3 };

        public Team GetTeam() => team;

        public BattleMember GetBattleMember() => battleMember;

        public PlayerInteractions GetInteractions() => playerInteractions;

        public NavMeshAgent GetAgent() => agent;

        public CinemachineFreeLook GetOverworldCameraRig() => overworldCameraRig;

        public Controller GetController() => controller;

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);

            characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(FileName));

            overworldCameraRig.enabled = false;

            overworldGameObject = controller.gameObject;

            moveController.Setup();
            StartCoroutine(playerInteractions.Setup());

            yield break;
        }

        public void EnablePlayerControl() => controller.Enable();

        public void DisablePlayerControl() => controller.Disable();

        // ReSharper disable once IdentifierTypo
        public void DisableOverworld()
        {
            if (overworldGameObject != null)
                overworldGameObject.SetActive(false);
        }

        // ReSharper disable once IdentifierTypo
        public void EnableOverworld()
        {
            if (overworldGameObject != null)
                overworldGameObject.SetActive(true);
        }

        public void PlayAnimationClip(AnimationClip clip)
        {
        }

        #endregion

        #region Internal

        #region Input

        private void OnMoveAxisChange(Vector2 vec) => moveDirection.value = vec;

        private void OnTurnAxisChange(Vector2 vec) => rotationDirection.value = vec;

        private void OnRunChange(bool set) => running.value = set;

        #endregion

        #endregion
    }
}