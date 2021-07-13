#region SDK

using System.Collections.Generic;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Item;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.States;
using Mfknudsen.Battle.UI;
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

    public enum MasterState
    {
        Setup,
        Starting,
        ChoosingMove,
        AI,
        Checking,
        Action,
        RoundDone,
        SelectNew
    }

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

        // ReSharper disable once NotAccessedField.Local
        private State stateManage;

        [SerializeField] private float secondsPerPokemonMove = 1;

        [SerializeField] private BattleDisplay display;

        [SerializeField] private GameObject turnDisplay;

        [SerializeField] private TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];
        
        [SerializeField] private SelectionMenu selectionMenu;
        
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

        private void Update()
        {
            /*
            if (!active)
                return;

            if (!isFainted)
            {
                switch (state)
                {
                    case MasterState.Setup:
                        Setup();
                        break;

                    case MasterState.Starting:
                        Starting();
                        break;

                    case MasterState.ChoosingMove:
                        ChoosingMove();
                        break;

                    case MasterState.AI:
                        AI();
                        break;

                    case MasterState.Checking:
                        Checking();
                        break;

                    case MasterState.Action:
                        Action();
                        break;

                    case MasterState.RoundDone:
                        RoundDone();
                        break;

                    case MasterState.SelectNew:
                        SelectNew();
                        break;
                }
            }
            else if (isFainted)
                DoFaintedPokemon();
                */
        }

        #endregion

        #region States

        private void SelectNew()
        {
            /*
            if (needNew == null)
            {
                foreach (Spot s in spots)
                {
                    if (s == null)
                        continue;

                    if (s.GetActive())
                        continue;

                    if (!s.GetNeedNew())
                        continue;

                    foreach (BattleMember m in members)
                    {
                        if (m == null)
                            continue;

                        if (m.OwnSpot(s) && m.GetTeam().CanSendMorePokemon())
                        {
                            s.SetActive(true);

                            if (m.IsPlayer())
                            {
                                BattleLog.instance.AddNewLog(name, "Waiting For Player Pokemon Selection");
                                pokemonSelectionMenu.SetActive(true);
                                pokemonMenu.SetFieldNames(m.GetTeam(), SelectorGoal.Switch);
                            }
                            else
                            {
                                pokemonSelectionMenu.SetActive(false);
                            }

                            needNew = s;
                            break;
                        }
                    }

                    if (needNew != null)
                        break;
                }

                if (needNew == null)
                {
                    pokemonSelectionMenu.SetActive(false);
                    chooseNew = false;
                    SwitchState(MasterState.Starting);
                }
            }
            */
        }

        private void DoFaintedPokemon()
        {
            /*
            foreach (Pokemon p in faintedPokemon)
            {
                if (checking == null)
                {
                    //chooseNew = true;
                    checking = p.GetConditionOversight();
                    conditionChecker = StartCoroutine(p.GetConditionOversight().CheckFaintedCondition());
                }
            }

            if (conditionChecker != null)
            {
                if (checking.GetDone())
                {
                    Pokemon p = checking.GetNonVolatileStatus().GetAffectedPokemon();

                    if (p.GetBattleAction() != null)
                        actionList.Remove(p.GetBattleAction());

                    faintedPokemon.Remove(p);
                    checking = null;
                    conditionChecker = null;
                }
            }

            if (faintedPokemon.Count == 0)
            isFainted = false;
            */
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

        #endregion

        #region In

        public void StartBattle(BattleStarter bs, BattleMember[] players)
        {
            starter = bs;
            members.Clear();

            string s = "Starting Battle Between:";
            foreach (BattleMember member in players)
            {
                if (!members.Contains(member))
                {
                    members.Add(member);
                    s += "\n    - " + member.GetName();
                }
            }

            BattleLog.instance.AddNewLog(name, s);

            SetState(new BeginState(this));
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            BattleLog.instance.AddNewLog(name, "Spawning: " + pokemon.GetName());

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
        }

        public SpotOversight SetupSpotOversight()
        {
            spotOversight = new SpotOversight();

            return spotOversight;
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
                BattleMember battleMember = MasterPlayer.instance.GetBattleMember();

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

        #region Display

        public void DisplayMoves(Pokemon pokemon)
        {
            if (!turnDisplay.activeSelf)
                turnDisplay.SetActive(true);

            PokemonMove[] toDisplay = pokemon.GetMoves();
            for (int i = 0; i < 4; i++)
            {
                Image img = buttonsText[i].transform.parent.GetComponent<Image>();

                img.color = Color.white;

                if (toDisplay[i] != null)
                {
                    buttonsText[i].text = toDisplay[i].GetName();
                    img.color = toDisplay[i].GetMoveType().GetTypeColor();
                }
                else
                    buttonsText[i].text = "";
            }
        }
        
        public void DisplayPokemonSelect()
        {
            selectionMenu.DisplaySelection(SelectorGoal.Switch);
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

        #endregion
    }
}