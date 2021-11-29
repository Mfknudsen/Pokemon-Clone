#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Settings.Manager;
using Mfknudsen.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;

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
        private BattleMember battleMember;

        [FoldoutGroup("References")] [SerializeField]
        private Interactions interactions;

        [FoldoutGroup("References")] [SerializeField]
        private Controller moveController;

        // ReSharper disable once IdentifierTypo
        private GameObject overworldGameObject;
        private PlayerInputContainer playerInputContainer;

        #region Character Sheet

        [FoldoutGroup("Character Sheet"), SerializeField]
        private int badgeCount;

        [FoldoutGroup("Character Sheet/Pronouns")]
        [HorizontalGroup("Character Sheet/Pronouns/Horizontal")]
        [HideLabel]
        [BoxGroup("Character Sheet/Pronouns/Horizontal/1", false), SerializeField]
        private string pronoun1 = "They";

        [HideLabel] [BoxGroup("Character Sheet/Pronouns/Horizontal/2", false), SerializeField]
        private string pronoun2 = "Them";

        [HideLabel] [BoxGroup("Character Sheet/Pronouns/Horizontal/3", false), SerializeField]
        private string pronoun3 = "Theirs";

        #endregion

        #endregion

        #region Getters

        public string[] GetPronouns()
        {
            return new[] { pronoun1, pronoun2, pronoun3 };
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

        #endregion

        #region Setters

        public void SetPronouns(string one, string two, string three)
        {
            if (!one.Equals(""))
                pronoun1 = one;
            if (!two.Equals(""))
                pronoun2 = two;
            if (!three.Equals(""))
                pronoun3 = three;
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

            InputManager.instance.moveAxisInputEvent.AddListener(OnMoveAxisChange);
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

        #endregion

        #endregion
    }
}