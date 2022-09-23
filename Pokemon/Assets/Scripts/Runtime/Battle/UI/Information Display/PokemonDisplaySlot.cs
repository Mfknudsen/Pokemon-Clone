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
            this.healthDisplay.gameObject.SetActive(!this.hideHealthText);
        }

        private void Update()
        {
            if (this.pokemon == null) return;

            float healthToDisplay = this.pokemon.GetCurrentHealth();

            if (healthToDisplay is < 1 and > 0)
                healthToDisplay = 1;

            this.healthDisplay.text = this.maxHPStat + " / " + (int) healthToDisplay;

            this.healthBar.SetCurrentBar(healthToDisplay);
        }

        #endregion

        #region Getters

        public bool GetActive()
        {
            return this.active;
        }


        public Vector3 GetOriginPosition()
        {
            return this.originPosition;
        }

        #endregion

        #region Setters

        public void SetOriginPosition(Vector3 pos)
        {
            this.originPosition = pos;
        }

        #endregion

        #region In

        public void Setup()
        {
            this.originPosition = gameObject.transform.position;
        }

        // ReSharper disable once ParameterHidesMember
        public void SetPokemon(Pokemon pokemon)
        {
            if (pokemon == null)
            {
                this.active = false;
                return;
            }

            this.maxHPStat = pokemon.GetMaxHealth();
            this.healthBar.SetBarMax(this.maxHPStat);
            this.pokemon = pokemon;
            this.nameDisplay.text = pokemon.GetName() + " " + pokemon.GetLevel() + "Lv";

            this.active = true;
        }

        #endregion
    }
}