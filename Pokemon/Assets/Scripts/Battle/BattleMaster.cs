#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//Custom
using Trainer;
using BattleUI;
#endregion

#region Enums
public enum MasterState { Setup, Starting, ChoosingMove, AI, Checking, Action, RoundDone, SelectNew }

public enum Weather { Rain, HarshSunlight, Hail }
#endregion

public class BattleMaster : MonoBehaviour
{
    #region Values
    public static BattleMaster instance;
    public MasterState state = 0;
    public static Weather weather = 0;

    [Header("Members:")]
    [SerializeField] private BattleMember[] members = new BattleMember[2];

    [Header("Pokemons:")]
    [SerializeField] private Pokemon[] pokemonOnSpots = new Pokemon[2];
    [SerializeField] private BattleAction[] pokemonActions = new BattleAction[2];

    [Header("Transforms")]
    [SerializeField] private Transform[] spawnPoints = new Transform[2];

    [Header("States:")]
    string splacerholder;

    [Header(" -- SetUp:")]
    int iplaceholder;

    [Header(" -- Action Selection:")]
    float fplaceholder;
    [SerializeField] private SwitchAction switchAction = null;

    [Header(" -- Checking:")]
    [SerializeField] private List<BattleAction> instantActions = new List<BattleAction>();
    [SerializeField] private List<BattleAction> fastActions = new List<BattleAction>(), normalActions = new List<BattleAction>(), slowActions = new List<BattleAction>();

    /////
    /////
    [Header(" -- Actions:")]
    [SerializeField] private int actionIndex = 0;
    [SerializeField] private List<BattleAction> combinedActions = new List<BattleAction>();
    [SerializeField] private float secondsPerPokemonMove = 1;
    Coroutine actionOperation = null;
    [SerializeField] BattleAction action = null;
    [SerializeField] private int actionSwitchState = 1;
    /////
    /////

    [Header(" -- Select New:")]
    Vector3 v3placeholder;

    [Header("Conditions:")]
    [SerializeField] private ConditionOversight checking = null;
    private Coroutine conditionChecker = null;
    private Coroutine conditionOperation = null;

    [Header("UI:")]
    [SerializeField] private BattleDisplay display = null;
    [SerializeField] private PokemonDisplay[] pokemonDisplay = new PokemonDisplay[2];
    [Header(" -- Turn Display:")]
    [SerializeField] private GameObject turnDisplay = null;
    [SerializeField] private TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];
    [Header(" -- Selectio Menu:")]
    [SerializeField] private GameObject selectionMenu = null;
    [SerializeField] private SelectionMenu menu = null;
    [Header(" -- Effective")]
    [SerializeField] private Chat superEffective = null;
    [SerializeField] private Chat notEffective = null, noEffect = null, barelyEffective = null, extremlyEffective = null;


    [Header("Chat:")]
    [SerializeField] private Chat SwitchChat = null;

    private void OnValidate()
    {
        actionSwitchState = 1;
    }
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

        state = MasterState.Setup;
    }

    private void Update()
    {
        if (members[0] == null)
            members[0] = Player.MasterPlayer.instance.gameObject.GetComponent<BattlePlayer>();

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
    #endregion

    #region States
    private void Setup()
    {
        ChatMaster.instance.Add((members[1] as BattleEnemy).GetStartChat());

        state = MasterState.Starting;
    }

    private void Starting()
    {
        if (ChatMaster.instance.GetIsClear())
        {
            for (int i = 0; i < members.Length; i++)
            {
                BattleMember member = members[i];

                if (!member.GetTeam().GetReady())
                    member.GetTeam().Setup();

                pokemonOnSpots[i] = member.GetTeam().GetPokemonByIndex(0);
                pokemonDisplay[i].SetNewPokemon(pokemonOnSpots[i]);
                pokemonOnSpots[i].SpawnPokemon(spawnPoints[i], true);
            }

            state = MasterState.ChoosingMove;
        }
    }

    private void ChoosingMove()
    {
        if (!turnDisplay.activeSelf)
        {
            turnDisplay.SetActive(true);
            DisplayMovesForPokemon();
        }

        if (pokemonActions[0] != null)
            state = MasterState.AI;
    }

    private void AI()
    {
        //
        // For TEST without AI
        if (pokemonActions[1] == null)
        {
            pokemonActions[1] = pokemonOnSpots[1].GetMoveByIndex(Random.Range(0, 3));

            if (pokemonActions[1] != null)
            {
                (pokemonActions[1] as PokemonMove).SetCurrentPokemon(pokemonOnSpots[1]);
                (pokemonActions[1] as PokemonMove).SetTargetPokemon(new Pokemon[] { pokemonOnSpots[0] });
            }
        }

        if (pokemonActions[1] != null)
            state = MasterState.Checking;
    }

    private void Checking()
    {
        turnDisplay.SetActive(false);

        CheckActions();

        state = MasterState.Action;
    }

    private void Action()
    {
        switch (actionSwitchState)
        {
            case 1:
                //Setup
                if (actionIndex < combinedActions.Count)
                {
                    action = combinedActions[actionIndex];
                    actionSwitchState = 2;

                    checking = null;
                    actionOperation = null;
                }
                else
                {
                    Debug.Log("No action to perform");
                    actionSwitchState = 7;
                }
                break;

            case 2:
                //Check Pokemon Can Attack
                if ((action as PokemonMove) != null)
                {
                    if (checking == null)
                    {
                        checking = action.GetCurrentPokemon().GetConditionOversight();
                        conditionChecker = StartCoroutine(checking.CheckConditionBeforeMove());
                    }
                    else if (checking != null)
                    {
                        if (ChatMaster.instance.GetIsClear() && checking.GetDone())
                        {
                            checking = null;
                            conditionChecker = null;

                            actionSwitchState = 3;
                        }
                    }
                }
                else if ((action as SwitchAction) != null)
                    actionSwitchState = 3;
                break;

            case 3:
                //Activation
                if (actionOperation == null)
                    actionOperation = StartCoroutine(action.Activate());

                if (actionOperation == null)
                {
                    Debug.Log("No operation in action");
                    actionSwitchState = 7;
                }
                else
                    actionSwitchState = 4;
                break;

            case 4:
                //Checking
                if (action.GetDone() && ChatMaster.instance.GetIsClear())
                {
                    actionSwitchState = 5;
                }
                break;

            case 5:
                //Moving on
                if (actionIndex < combinedActions.Count)
                {
                    actionIndex++;

                    if (actionIndex == combinedActions.Count || (pokemonOnSpots[0].GetCurrentHealth() == 0 || pokemonOnSpots[1].GetCurrentHealth() == 0))
                    {
                        foreach (Pokemon member in pokemonOnSpots)
                            member.GetConditionOversight().Reset();

                        actionSwitchState = 6;
                    }
                    else
                        actionSwitchState = 1;
                }
                break;

            case 6:
                //Conditions
                if (conditionChecker != null && checking != null)
                {
                    if (checking.GetDone())
                        conditionChecker = null;
                }

                if (conditionChecker == null)
                {
                    foreach (Pokemon pokemon in pokemonOnSpots)
                    {
                        if (!pokemon.GetConditionOversight().GetDone())
                        {
                            checking = pokemon.GetConditionOversight();
                            conditionChecker = StartCoroutine(checking.CheckConditionEndTurn());
                        }
                    }
                }

                if (conditionChecker == null && checking.GetDone())
                {
                    conditionChecker = null;
                    checking = null;
                    foreach (Pokemon pokemon in pokemonOnSpots)
                        pokemon.GetConditionOversight().SetDone(false);

                    actionSwitchState = 7;
                }

                break;

            case 7:
                actionIndex = 0;
                actionSwitchState = 1;
                action = null;
                actionOperation = null;

                for (int i = 0; i < pokemonOnSpots.Length; i++)
                    pokemonActions[i] = null;

                //Clear
                slowActions.Clear();
                normalActions.Clear();
                fastActions.Clear();
                instantActions.Clear();
                combinedActions.Clear();

                if (pokemonOnSpots[0].GetCurrentHealth() == 0 || pokemonOnSpots[1].GetCurrentHealth() == 0)
                    state = MasterState.RoundDone;
                else
                    state = MasterState.ChoosingMove;

                //Reset and end Action State!
                break;
        }
    }

    private void RoundDone()
    {
        if (pokemonOnSpots[0].GetCurrentHealth() == 0)
        {
            Debug.Log("You Lose!");
            pokemonOnSpots[0] = null;
        }
        else if (pokemonOnSpots[1].GetCurrentHealth() == 0)
        {
            Debug.Log("You Win!");
            pokemonOnSpots[1] = null;
        }

        state = MasterState.SelectNew;
    }

    private void SelectNew()
    {
        if (pokemonOnSpots[1] != null)
        {
            if (pokemonOnSpots[1].GetCurrentHealth() == 0)
            {
                Debug.Log("Despawning Enemy Pokemon");
                pokemonOnSpots[1].DespawnPokemon();
            }
        }

        if (WorldMaster.instance != null)
        {
            if (WorldMaster.instance.GetEmpty())
            {
                //WorldMaster.instance.UnloadCurrentBattleScene();
                //Debug.Log("Unloading");
            }
        }
    }
    #endregion

    #region Getters
    public float GetSecPerPokeMove()
    {
        return secondsPerPokemonMove;
    }
    public Coroutine GetConditionOperation()
    {
        return conditionOperation;
    }
    #endregion

    #region Setters
    public void SetConditionOperation(IEnumerator toStart)
    {
        if (toStart != null)
            conditionOperation = StartCoroutine(toStart);
        else
            conditionOperation = null;
    }
    #endregion

    #region In
    public void CheckOnAction()
    {
        PokemonMove m = normalActions[actionIndex] as PokemonMove;

        if (m.GetDone())
        {
            //Update();
        }
    }

    public void SetNewActivePokemonByIndex(int index)
    {

    }
    #endregion

    #region Internal
    public void StartBattle(BattleMember player, BattleMember[] enemies)
    {
        members[0] = player;

        for (int i = 0; i < enemies.Length; i++)
            members[i + 1] = enemies[i];
    }

    private void EndBattle()
    {

    }

    private void DisplayMovesForPokemon()
    {
        PokemonMove[] toDisplay = pokemonOnSpots[0].GetMoves();

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

    private void CheckActions()
    {
        if ((pokemonActions[0] as PokemonMove) != null)
        {
            normalActions.Add(pokemonActions[0]);
        }
        else
        {
            instantActions.Add(pokemonActions[0]);
        }

        if (pokemonOnSpots[0].GetStat(Stat.Speed) < pokemonOnSpots[1].GetStat(Stat.Speed))
        {
            normalActions.Insert(0, pokemonActions[1]);
        }
        else
        {
            normalActions.Add(pokemonActions[1]);
        }

        combinedActions.AddRange(instantActions);
        combinedActions.AddRange(fastActions);
        combinedActions.AddRange(normalActions);
        combinedActions.AddRange(slowActions);
    }

    public void SelectAction(int i)
    {
        if (i > 0 && i < 5)
        {
            PokemonMove move = pokemonOnSpots[0].GetMoveByIndex(i - 1);
            if (move != null)
            {
                move.SetCurrentPokemon(pokemonOnSpots[0]);
                move.SetTargetPokemon(new Pokemon[] { pokemonOnSpots[1] });
                pokemonActions[0] = move;
            }
        }
        else if (i == 5)
        {
            selectionMenu.SetActive(true);
        }
        else if (i == 6)
        {
            Debug.Log("Bag Not Implemented!");
            return;
        }
        else
        {
            Debug.Log("Run Not Implemented!");
            return;
        }
    }

    public void SelectNewPokemon(int i)
    {
        if (i > 0 && i < 6)
        {
            Pokemon result = members[0].GetTeam().GetPokemonByIndex(i);

            if (result != null)
            {
                SwitchAction action = switchAction.GetAction() as SwitchAction;

                action.SetCurrentPokemon(members[0].GetTeam().GetPokemonByIndex(0));
                action.SetNextPokemon(members[0].GetTeam().GetPokemonByIndex(i));

                members[0].GetTeam().SwitchTeamPlaces(0, i);

                pokemonOnSpots[0].SpawnPokemon(spawnPoints[0], true);

                pokemonActions[0] = action;


                selectionMenu.SetActive(false);
            }
        }
    }
    #endregion
}