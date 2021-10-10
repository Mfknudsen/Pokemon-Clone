#region Packages

using System.Linq;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.Systems.States;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

// ReSharper disable once ParameterTypeCanBeEnumerable.Global
namespace Mfknudsen.Battle.Systems
{
    public class BattleManager : MonoBehaviour
    {
        #region Values

        public static BattleManager instance;
        [SerializeField] private BattleStarter starter;
        [SerializeField] private Condition faintCondition;
        [SerializeField] private BattleAction switchAction, itemAction;

        [SerializeField] private GameObject spotPrefab;

        private WeatherManager weatherManager;
        private SpotOversight spotOversight;
        private AbilityOversight abilityOversight;

        // ReSharper disable once NotAccessedField.Local
        private State stateManage;

        [SerializeField] private float secondsPerPokemonMove = 1;

        [SerializeField] private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;

        [SerializeField] private Chat superEffective;

        [SerializeField] private Chat notEffective, noEffect, barelyEffective, extremelyEffective, miss;

        #endregion

        #region Build In States

        private void Start()
        {
            instance = this;
            
            BattleMathf.SetSuperEffective(superEffective);
            BattleMathf.SetNotEffective(notEffective);
            BattleMathf.SetNoEffect(noEffect);
            BattleMathf.SetBarelyEffective(barelyEffective);
            BattleMathf.SetExtremlyEffective(extremelyEffective);
            BattleMathf.SetMissChat(miss);
        }

        #endregion

        #region Getters

        public SelectionMenu GetSelectionMenu()
        {
            return selectionMenu;
        }

        public float GetSecPerPokeMove()
        {
            return secondsPerPokemonMove;
        }

        public BattleStarter GetStarter()
        {
            return starter;
        }

        public WeatherManager GetWeatherManager()
        {
            return weatherManager ??= new WeatherManager();
        }

        public SpotOversight GetSpotOversight()
        {
            return spotOversight ??= new SpotOversight();
        }

        public AbilityOversight GetAbilityOversight()
        {
            return abilityOversight;
        }

        public DisplayManager GetDisplayManager()
        {
            return displayManager;
        }

        #endregion

        #region Setters

        public void SetDisplayManager(DisplayManager set)
        {
            displayManager = set;
        }

        public void SetSelectionMenu(SelectionMenu set)
        {
            selectionMenu = set;
        }

        #endregion

        #region In

        public void StartBattle(BattleStarter bs)
        {
            starter = bs;

            SetState(new BeginState(this));
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            BattleLog.AddLog(name, "Spawning: " + pokemon.GetName());

            Transform trans = spot.GetTransform();
            GameObject obj = Instantiate(pokemon.GetPokemonPrefab(), trans, true);

            obj.transform.position = trans.position;
            obj.transform.rotation = trans.rotation;

            pokemon.SetSpawnedObject(obj);
            pokemon.SetInBattle(true);
            pokemon.SetGettingSwitched(false);
            pokemon.SetRevived(false);

            spot.SetActivePokemon(pokemon);
            spot.SetNeedNew(false);

            displayManager.UpdateSlots();

            //Check if spawned object is placeholder;
            PokemonPlaceholder.CheckPlaceholder(pokemon, obj);
        }

        // ReSharper disable once IdentifierTypo
        public void DespawnPokemon(Pokemon pokemon)
        {
            if (pokemon == null) return;

            foreach (Spot s in spotOversight.GetSpots()
                // ReSharper disable once Unity.NoNullPropagation
                .Select(s => new { s, p = s?.GetActivePokemon() })
                .Where(t => t.p is { })
                .Where(t => t.p == pokemon)
                .Select(t => t.s))
            {
                pokemon.DespawnPokemon();

                s.SetActivePokemon(null);
                
                break;
            }

            displayManager.UpdateSlots();
        }

        public void SetupAbilityOversight()
        {
            abilityOversight = new AbilityOversight();

            abilityOversight.Setup();
        }

        public void SetState(State s)
        {
            stateManage = s;
            StartCoroutine(s.Tick());
        }

        public void EndBattle(bool playerVictory)
        {
            starter.EndBattle(playerVictory);
        }

        public void SetPokemonFainted(Pokemon pokemon)
        {
            FaintedCondition condition = faintCondition.GetCondition() as FaintedCondition;

            // ReSharper disable once PossibleNullReferenceException
            condition.SetAffectedPokemon(pokemon);

            pokemon.GetConditionOversight().TryApplyNonVolatileCondition(condition);
        }

        #endregion

        #region Out

        public GameObject CreateSpot()
        {
            return Instantiate(spotPrefab);
        }

        public SwitchAction InstantiateSwitchAction()
        {
            return (SwitchAction)Instantiate(switchAction);
        }

        public ItemAction InstantiateItemAction()
        {
            return Instantiate(itemAction) as ItemAction;
        }

        #endregion
    }
}