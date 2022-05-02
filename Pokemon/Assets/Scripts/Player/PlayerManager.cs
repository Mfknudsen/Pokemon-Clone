#region Packages

using System.Collections;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Files;
using Mfknudsen.Items;
using Mfknudsen.Settings.Manager;
using Mfknudsen.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.Player
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

        private GameObject overworldGameObject;
        private PlayerInputContainer playerInputContainer;

        [FoldoutGroup("Character Sheet"), HideLabel] [SerializeField]
        private CharacterSheet characterSheet;

        private static readonly string fileName = "PlayerData";

        #endregion

        #region Getters

        public CharacterSheet GetCharacterSheet()
        {
            return characterSheet;
        }

        public string[] GetPronouns()
        {
            return new[] { characterSheet.pronoun1, characterSheet.pronoun2, characterSheet.pronoun3 };
        }

        public Team GetTeam()
        {
            return team;
        }

        public BattleMember GetBattleMember()
        {
            return battleMember;
        }

        public PlayerInteractions GetInteractions()
        {
            return playerInteractions;
        }

        public PlayerInputContainer GetPlayerInput()
        {
            return playerInputContainer;
        }

        public NavMeshAgent GetAgent()
        {
            return agent;
        }

        public CinemachineFreeLook GetOverworldCameraRig()
        {
            return overworldCameraRig;
        }

        public Controller GetController()
        {
            return controller;
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);

            characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(fileName));

            overworldCameraRig.enabled = false;

            overworldGameObject = controller.gameObject;

            playerInputContainer = new PlayerInputContainer();

            moveController.Setup();
            StartCoroutine(playerInteractions.Setup());

            InputManager inputManager = InputManager.Instance;
            inputManager.moveAxisInputEvent.AddListener(OnMoveAxisChange);
            inputManager.runInputEvent.AddListener(OnRunChange);

            yield break;
        }

        public void EnablePlayerControl()
        {
            controller.Enable();
        }

        public void DisablePlayerControl()
        {
            controller.Disable();
        }

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

        #endregion

        #region Internal

        #region Input

        private void OnMoveAxisChange(Vector2 vec)
        {
            playerInputContainer.SetMoveDirection(vec);
        }

        private void OnRunChange(bool set)
        {
            playerInputContainer.SetRun(set);
        }

        #endregion

        #endregion
    }
}