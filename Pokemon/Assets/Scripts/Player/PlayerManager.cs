#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Settings.Manager;
using Mfknudsen.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.Player
{
    [RequireComponent(typeof(BattleMember),
        typeof(Team), typeof(Inventory))]
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
        private Interactions interactions;

        [FoldoutGroup("References")] [SerializeField]
        private Controller moveController;

        // ReSharper disable once IdentifierTypo
        private GameObject overworldGameObject;
        private PlayerInputContainer playerInputContainer;

        [SerializeField] private CharacterSheet characterSheet;

        #endregion

        #region Getters

        public CharacterSheet GetCharacterSheet()
        {
            return characterSheet;
        }

        public string[] GetPronouns()
        {
            return new[] {characterSheet.pronoun1, characterSheet.pronoun2, characterSheet.pronoun3};
        }

        public Team GetTeam()
        {
            return team;
        }

        public BattleMember GetBattleMember()
        {
            return battleMember;
        }

        public Interactions GetInteractions()
        {
            return interactions;
        }

        public PlayerInputContainer GetPlayerInput()
        {
            return playerInputContainer;
        }

        public NavMeshAgent GetAgent()
        {
            return agent;
        }

        #endregion

        #region In

        public override void Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            overworldGameObject = controller.gameObject;
            overworldGameObject.SetActive(false);

            playerInputContainer = new PlayerInputContainer();

            moveController.Setup();
            interactions.Setup();

            InputManager inputManager = InputManager.instance;
            inputManager.moveAxisInputEvent.AddListener(OnMoveAxisChange);
            inputManager.runInputEvent.AddListener(OnRunChange);
        }

        // ReSharper disable once IdentifierTypo
        public void DisableOverworld()
        {
            overworldGameObject.SetActive(false);
        }

        // ReSharper disable once IdentifierTypo
        public void EnableOverworld()
        {
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