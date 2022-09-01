﻿#region Packages

using System.Collections;
using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Files;
using Runtime.Items;
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

        private GameObject overworldGameObject;
        private PlayerInputContainer playerInputContainer;

        [FoldoutGroup("Character Sheet"), HideLabel] [SerializeField]
        private CharacterSheet characterSheet;

        private const string FileName = "PlayerData";

        #endregion

        #region Build In State

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.AddListener(OnMoveAxisChange);
            inputManager.turnAxisInputEvent.AddListener(OnTurnAxisChange);
            inputManager.runInputEvent.AddListener(OnRunChange);
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

            characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(FileName));

            overworldCameraRig.enabled = false;

            overworldGameObject = controller.gameObject;

            playerInputContainer = new PlayerInputContainer();

            moveController.Setup();
            StartCoroutine(playerInteractions.Setup());

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

        public void PlayAnimationClip(AnimationClip clip)
        {
        }

        #endregion

        #region Internal

        #region Input

        private void OnMoveAxisChange(Vector2 vec) => playerInputContainer.moveDir = vec;

        private void OnTurnAxisChange(Vector2 vec) => playerInputContainer.rotDir = vec;

        private void OnRunChange(bool set) => playerInputContainer.run = set;

        #endregion

        #endregion
    }
}