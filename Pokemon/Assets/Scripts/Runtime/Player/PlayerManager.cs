#region Packages

using System;
using System.Collections;
using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Common;
using Runtime.Files;
using Runtime.ScriptableEvents;
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

        [ShowInInspector, ReadOnly, NonSerialized]
        private PlayerState playerState = PlayerState.Paused;

        [FoldoutGroup("References")] [SerializeField]
        private Team team;

        [FoldoutGroup("References")] [SerializeField]
        private Controller controller;

        [FoldoutGroup("References")] [SerializeField]
        private NavMeshAgent agent;

        [FoldoutGroup("References")] [SerializeField]
        private BattleMember battleMember;

        [FoldoutGroup("References")] [SerializeField]
        private PlayerInteraction playerInteraction;

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

        [SerializeField, BoxGroup("Events"), Required]
        private PlayerStateEvent playerStateEvent;

        private GameObject playerGameObject;

        private const string FileName = "PlayerData";

        #endregion

        #region Build In State

        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator FrameStart(PersistantRunner runner)
        {
            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.AddListener(this.OnMoveAxisChange);
            inputManager.turnAxisInputEvent.AddListener(this.OnTurnAxisChange);
            inputManager.runInputEvent.AddListener(this.OnRunChange);

            this.playerGameObject = GameObject.Find("Player");

            DontDestroyOnLoad(this.playerGameObject);

            GameObject overworld = this.playerGameObject.GetChildByName("Overworld");

            this.overworldCameraRig = overworld.GetFirstComponentByRoot<CinemachineFreeLook>();

            this.agent = overworld.GetComponent<NavMeshAgent>();

            this.team = this.playerGameObject.GetComponent<Team>();

            this.defaultOverworldRig.value = this.overworldCameraRig;

            this.controller = overworld.GetComponent<Controller>();

            this.playerInteraction = overworld.GetComponent<PlayerInteraction>();

            this.cameraBrain.value = this.playerGameObject.GetComponentInChildren<CinemachineBrain>();

            this.characterSheet = new CharacterSheet(FileManager.LoadData<PlayerData>(FileName));
            this.overworldCameraRig.enabled = false;
            this.controller.Setup();

            this.battleMember = this.playerGameObject.GetFirstComponentByRoot<BattleMember>();

            this.ready = true;

            yield break;
        }

        #endregion

        #region Getters

        public CharacterSheet GetCharacterSheet() =>
            this.characterSheet;

        public string[] GetPronouns() => new[]
            { this.characterSheet.pronoun1, this.characterSheet.pronoun2, this.characterSheet.pronoun3 };

        public Team GetTeam() =>
            this.team;

        public BattleMember GetBattleMember() =>
            this.battleMember;

        public PlayerInteraction GetInteractions() =>
            this.playerInteraction;

        public NavMeshAgent GetAgent() =>
            this.agent;

        public CinemachineFreeLook GetOverworldCameraRig() =>
            this.overworldCameraRig;

        public Controller GetController() =>
            this.controller;

        public PlayerState GetPlayerState() =>
            this.playerState;

        #endregion

        #region Setters

        public void SetState(PlayerState set)
        {
            this.playerState = set;
            this.playerStateEvent.Trigger(set);
        }

        #endregion

        #region In

        public void EnablePlayerControl()
        {
            this.playerInteraction.enabled = true;
            this.controller.Enable();
        }

        public void DisablePlayerControl()
        {
            this.playerInteraction.enabled = false;
            this.controller.Disable();
        }

        public void DisableOverworld() => this.playerGameObject.SetActive(false);

        public void EnableOverworld() => this.playerGameObject.SetActive(true);

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