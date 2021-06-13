using Mfknudsen.Trainer;
using UnityEngine;

namespace Mfknudsen.Player
{
    public class MasterPlayer : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [HideInInspector] public static MasterPlayer instance = null;
        [SerializeField] private Team team;
        [SerializeField] private Controller controller;

        [Header("Character Sheet:")]
        [SerializeField] private int bagdeCount = 0;
        [SerializeField] private string[] pronouns = new string[2] { "They", "Them" }; //Inspired by Temtem
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
            return pronouns;
        }
        #endregion

        #region Setters
        public void SetPronous(string one, string two)
        {
            if (one != "")
                pronouns[0] = one;
            else if (two != "")
                pronouns[1] = two;
        }
        #endregion
    }
}
