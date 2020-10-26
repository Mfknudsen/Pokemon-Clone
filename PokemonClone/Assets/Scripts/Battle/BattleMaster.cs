using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;
using BattleUI;

public enum MasterState { Starting, ChoosingMove, Checking, Attacking, SelectNew }

public class BattleMaster : MonoBehaviour
{
    public static BattleMaster Master;
    public MasterState state = 0;

    [Header("Members:")]
    public BattleMember player = null;
    public BattleMember enemy = null;

    [Header("Pokemons:")]
    public Pokemon playerPokemon = null;
    public Pokemon enemyPokemon = null;

    [Header("UI:")]
    [SerializeField]
    public BattleDisplay display = null;
    public PokemonDisplay playerPokemonDisplay = null, enemyPokemonDisplay = null;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (Master == null)
            Master = this;
        else
            Destroy(gameObject);

        display.DisplayNewText("You are challenged by " + enemy.name + "!");
    }

    private void Update()
    {
        switch (state)
        {
            case MasterState.Starting:
                break;

            case MasterState.ChoosingMove:
                if (!player.selectMove)
                {
                    player.selectMove = true;
                    enemy.selectMove = true;
                }

                if (player.hasSelectedMove && enemy.hasSelectedMove)
                {
                    player.selectMove = false;
                    enemy.selectMove = false;
                }
                break;

            case MasterState.Checking:
                break;

            case MasterState.Attacking:
                break;

            case MasterState.SelectNew:
                break;

            default:
                Debug.LogError("Out of State!");
                break;
        }
    }
}
