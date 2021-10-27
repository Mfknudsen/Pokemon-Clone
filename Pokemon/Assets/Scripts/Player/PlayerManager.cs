#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.Player
{
    [RequireComponent(typeof(BattleMember), typeof(Team), typeof(Inventory))]
    public class PlayerManager : MonoBehaviour
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

        #region Character Sheet

        [FoldoutGroup("Character Sheet"), SerializeField]
        private int bagdeCount = 0;

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

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

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
    }
}