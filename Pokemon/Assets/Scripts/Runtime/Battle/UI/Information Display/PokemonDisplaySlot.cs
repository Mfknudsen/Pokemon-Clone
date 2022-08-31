#region Packages

using Runtime.Pokémon;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime.Battle.UI.Information_Display
{
    public class PokemonDisplaySlot : MonoBehaviour
    {
        #region Values

        [SerializeField] private bool hideHealthText;
        [SerializeField] private TextMeshProUGUI nameDisplay, healthDisplay;
        [SerializeField] private ProgressBar healthBar;

        // ReSharper disable once InconsistentNaming
        private int maxHPStat;
        private Pokemon pokemon;
        private bool active;
        private Vector3 originPosition;

        #endregion

        #region Build In States

        private void Start()
        {
            healthDisplay.gameObject.SetActive(!hideHealthText);
        }

        private void Update()
        {
            if (pokemon == null) return;

            float healthToDisplay = pokemon.GetCurrentHealth();

            if (healthToDisplay < 1 && healthToDisplay > 0)
                healthToDisplay = 1;

            healthDisplay.text = maxHPStat + " / " + (int) healthToDisplay;

            healthBar.SetCurrentBar(healthToDisplay);
        }

        #endregion

        #region Getters

        public bool GetActive()
        {
            return active;
        }


        public Vector3 GetOriginPosition()
        {
            return originPosition;
        }

        #endregion

        #region Setters

        public void SetOriginPosition(Vector3 pos)
        {
            originPosition = pos;
        }

        #endregion

        #region In

        public void Setup()
        {
            originPosition = gameObject.transform.position;
        }

        // ReSharper disable once ParameterHidesMember
        public void SetPokemon(Pokemon pokemon)
        {
            if (pokemon == null)
            {
                active = false;
                return;
            }

            maxHPStat = pokemon.GetMaxHealth();
            healthBar.SetBarMax(maxHPStat);
            this.pokemon = pokemon;
            nameDisplay.text = pokemon.GetName() + " " + pokemon.GetLevel() + "Lv";

            active = true;
        }

        #endregion
    }
}