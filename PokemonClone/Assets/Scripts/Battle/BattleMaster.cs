using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Trainer;
using BattleUI;

public enum MasterState { Starting, ChoosingMove, AI, Checking, Action, RoundDone, SelectNew }

public enum Weather { Rain, HarshSunlight, Hail }

public class BattleMaster : MonoBehaviour
{
    public static BattleMaster Master;
    public static MasterState state = 0;
    public static Weather weather = 0;

    [Header("Members:")]
    public BattleMember player = null;
    public BattleMember enemy = null;

    [Header("Pokemons:")]
    public Pokemon playerPokemon = null;
    public Pokemon enemyPokemon = null;
    private BattleAction playerAction = null;
    private BattleAction enemyAction = null;

    [Header("Transforms")]
    public Transform playerTransform = null;
    public Transform enemyTransform = null;

    [Header("States:")]
    string splacerholder;
    [Header(" -- SetUp:")]
    int iplaceholder;
    [Header(" -- Action Selection:")]
    float fplaceholder;
    [Header(" -- Checking:")]
    [SerializeField] private List<BattleAction> instantActions = new List<BattleAction>(), fastActions = new List<BattleAction>(), normalActions = new List<BattleAction>(), slowActions = new List<BattleAction>();
    [Header(" -- Actions:")]
    Vector2 v2placeholder;
    [Header(" -- Select New:")]
    Vector3 v3placeholder;

    [Header("UI:")]
    [SerializeField]
    public BattleDisplay display = null;
    public PokemonDisplay playerPokemonDisplay = null, enemyPokemonDisplay = null;
    [Header(" -- Turn Display:")]
    public GameObject turnDisplay = null;
    public TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];
    [Header(" -- Selectio Menu:")]
    public GameObject selectionMenu = null;

    private void Start()
    {
        if (Master == null)
            Master = this;
        else
            Destroy(gameObject);

        Setup();

        display.DisplayNewText("You are challenged by " + enemy.name + "!");
        selectionMenu.SetActive(false);

        state = MasterState.ChoosingMove;
    }

    private void Update()
    {
        switch (state)
        {
            case MasterState.Starting:
                break;

            case MasterState.ChoosingMove:
                if (!turnDisplay.activeSelf)
                {
                    turnDisplay.SetActive(true);
                    DisplayForPokemon();
                }

                if (playerAction != null)
                    state = MasterState.AI;
                break;

            case MasterState.AI:
                //
                // For TEST without AI
                if (enemyAction == null)
                {
                    enemyAction = Instantiate(enemyPokemon.GetMoveByIndex(0));
                    PokemonMove move = enemyAction as PokemonMove;
                    move.SetCurrentPokemon(enemyPokemon);
                    move.SetTargetPokemon(new Pokemon[] { playerPokemon });
                }
                state = MasterState.Checking;
                break;

            case MasterState.Checking:
                turnDisplay.SetActive(false);

                CheckActions();

                state = MasterState.Action;
                break;

            case MasterState.Action:
                foreach (PokemonMove action in normalActions)
                {
                    bool check = true;

                    if (action == playerAction && playerPokemon.GetCurrentHealth() == 0)
                        check = false;
                    else if (action == enemyAction && enemyPokemon.GetCurrentHealth() == 0)
                        check = false;

                    if (check)
                        action.Activate();
                }

                normalActions.Clear();
                enemyAction = null;
                playerAction = null;

                if (playerPokemon.GetCurrentHealth() == 0 || enemyPokemon.GetCurrentHealth() == 0)
                    state = MasterState.RoundDone;
                else
                    state = MasterState.ChoosingMove;
                break;

            case MasterState.RoundDone:
                Debug.Log("Done");

                if (playerPokemon.GetCurrentHealth() == 0)
                {
                    Debug.Log("You Lose!");
                    playerPokemon = null;
                }
                else if (enemyPokemon.GetCurrentHealth() == 0)
                {
                    Debug.Log("You Win!");
                    enemyPokemon = null;
                }

                state = MasterState.SelectNew;
                break;

            case MasterState.SelectNew:
                break;
        }
    }

    private void Setup()
    {
        playerPokemon = player.team.GetPokemonByIndex(0);
        playerPokemonDisplay.SetNewPokemon(playerPokemon);

        enemyPokemon = enemy.team.GetPokemonByIndex(0);
        enemyPokemonDisplay.SetNewPokemon(enemyPokemon);
    }

    private void DisplayForPokemon()
    {
        PokemonMove[] toDisplay = playerPokemon.GetMoves();
        for (int i = 0; i < 4; i++)
        {
            if (toDisplay[i] != null)
            {
                buttonsText[i].text = toDisplay[i].GetName();
            }
            else
                buttonsText[i].text = "";
        }
    }

    private void CheckActions()
    {
        if (enemyPokemon.GetSpeed() < playerPokemon.GetSpeed())
        {
            normalActions.Add(playerAction);
            normalActions.Add(enemyAction);
        }
        else
        {
            normalActions.Add(enemyAction);
            normalActions.Add(playerAction);
        }
    }

    public void SelectAction(int i)
    {
        if (i > 0 && i < 5)
        {
            playerAction = Instantiate(playerPokemon.GetMoveByIndex(i - 1));
            PokemonMove move = playerAction as PokemonMove;
            move.SetCurrentPokemon(playerPokemon);
            move.SetTargetPokemon(new Pokemon[] { enemyPokemon });
        }
        else if (i == 5)
        {
            Debug.Log("Switch Not Implemented!");
            selectionMenu.SetActive(true);
            return;
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
        Pokemon result = null;

        if (i > 0 && i < 6)
        {
            result = player.team.GetPokemonByIndex(i);

            if(result != null)
            {
                player.team.SwitchTeamPlaces(0, i);

                playerPokemon = Instantiate(player.team.GetPokemonByIndex(0));
                playerPokemonDisplay.SetNewPokemon(playerPokemon);

                Debug.Log(result.GetName());

                selectionMenu.SetActive(false);

                playerAction = playerPokemon.GetMoveByIndex(0);
            }
        }
    }
}
