#region SDK

using System.Collections.Generic;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Item;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.Systems.States;
using Mfknudsen.Battle.UI;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Comunication;
using Mfknudsen.Items;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

// ReSharper disable once ParameterTypeCanBeEnumerable.Global
namespace Mfknudsen.Battle.Systems
{
    #region Enums

    public enum Weather
    {
        None,
        Rain,
        HarshSunlight,
        Hail
    }

    #endregion

    public class BattleMaster : MonoBehaviour
    {
        #region Values

        public static BattleMaster instance;
        [SerializeField] private BattleStarter starter;
        [SerializeField] private Weather weather = 0;
        [SerializeField] private Condition faintCondition;
        [SerializeField] private BattleAction switchAction, itemAction;

        [SerializeField] private List<BattleMember> members = new List<BattleMember>();

        [SerializeField] private GameObject spotPrefab;

        private SpotOversight spotOversight;

        private AbilityOversight abilityOversight;

        // ReSharper disable once NotAccessedField.Local
        private State stateManage;

        [SerializeField] private float secondsPerPokemonMove = 1;
        
        [SerializeField] private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;
        
        [SerializeField] private Chat superEffective;

        [SerializeField] private Chat notEffective, noEffect, barelyEffective, extremlyEffective;

        #endregion

        #region Build In States

        private void Start()
        {
            instance = this;

            BattleMathf.SetSuperEffective(superEffective);
            BattleMathf.SetNotEffective(notEffective);
            BattleMathf.SetNoEffect(noEffect);
            BattleMathf.SetBarelyEffective(barelyEffective);
            BattleMathf.SetExtremlyEffective(extremlyEffective);
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

        public Weather GetWeather()
        {
            return weather;
        }

        public BattleStarter GetStarter()
        {
            return starter;
        }

        public SpotOversight GetSpotOversight()
        {
            return spotOversight;
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

        public void StartBattle(BattleStarter bs, BattleMember[] players)
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
            if (pokemon is null) return;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot s in spotOversight.GetSpots())
            {
                // ReSharper disable once Unity.NoNullPropagation
                Pokemon p = s?.GetActivePokemon();
                if (p is null)
                    continue;

                if (p != pokemon) continue;

                pokemon.DespawnPokemon();

                s.SetActivePokemon(null);
            }
            
            displayManager.UpdateSlots();
        }

        public SpotOversight SetupSpotOversight()
        {
            spotOversight = new SpotOversight();

            return spotOversight;
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

        #region Selection

        public void SelectAction(int i)
        {
            if (i > 0 && i < 5)
            {
                BattleMember battleMember = PlayerManager.instance.GetBattleMember();

                foreach (Spot spot in spotOversight.GetSpots())
                {
                    Pokemon pokemon = spot.GetActivePokemon();

                    if (!battleMember.GetTeam().PartOfTeam(pokemon))
                        continue;

                    PokemonMove move = pokemon.GetMoveByIndex(i - 1);

                    if (move == null)
                        return;

                    move.SetCurrentPokemon(pokemon);

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (Spot s in spotOversight.GetSpots())
                    {
                        if (s.Equals(spot) ||
                            s.GetActivePokemon() is null ||
                            battleMember.GetTeam().PartOfTeam(s.GetActivePokemon())) continue;

                        move.SetTargets(s.GetActivePokemon());

                        break;
                    }

                    pokemon.SetBattleAction(move);
                }
            }
            else if (i == 5)
            {
                //pokemonSelectionMenu.SetActive(true);

                foreach (BattleMember m in members)
                {
                    if (m.IsPlayer())
                    {
                        //pokemonMenu.SetFieldNames(m.GetTeam(), SelectorGoal.Switch);
                        break;
                    }
                }
            }
            else if (i == 6)
            {
                //itemSelectionMenu.SetActive(true);
                foreach (BattleMember m in members)
                {
                    if (m == null)
                        continue;
                    if (!m.IsPlayer())
                        continue;

                    //itemMenu.Setup(m.GetInventory().GetAllItems());
                    break;
                }
            }
            else
            {
                Debug.Log("Run Not Implemented!");
            }
        }

        public void SelectNewPokemon(int i)
        {
        }

        public void SelectItem(Item item, Pokemon target)
        {
            foreach (Spot s in spotOversight.GetSpots())
            {
                if (s == null)
                    continue;
                Pokemon p = s.GetActivePokemon();
                if (p == null)
                    continue;
                if (p.GetBattleAction() != null)
                    continue;

                // ReSharper disable once LocalVariableHidesMember
                ItemAction action = itemAction.GetAction() as ItemAction;
                // ReSharper disable once PossibleNullReferenceException
                action.SetToUse(item);
                action.SetCurrentPokemon(target);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (BattleMember m in members)
                {
                    if (!m.GetInventory().IsItemInBag(item))
                        continue;
                    action.SetBattleMember(m);
                }

                //actionInWait = action;
                //user = p;
                //waitForTarget = true;

                p.SetBattleAction(action);

                //pokemonSelectionMenu.SetActive(false);
                //itemSelectionMenu.SetActive(false);

                break;
            }
        }

        #endregion

        #endregion

        #region Out

        public GameObject CreateSpot()
        {
            return Instantiate(spotPrefab);
        }

        public SwitchAction InstantiateSwitchAction()
        {
            return (SwitchAction) Instantiate(switchAction);
        }

        public ItemAction InstantiateItemAction()
        {
            return Instantiate(itemAction) as ItemAction;
        }

        #endregion
    }
}