#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

#region Enums
public enum Stat { HP, Attack, Defence, SpAtk, SpDef, Speed }
public enum EvolutionMethod { None, Level, Item, Trade, LearnedMove }
public enum EggGroup { Monster, Water1 }
public enum LevelRate { Slow, MediumSlow }
public enum Shape { }
public enum Footprint { }
#endregion

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon", order = 0)]
public class Pokemon : ScriptableObject
{
    #region Values:
    #region Pokemon
    [Header("Object Reference:")]
    [SerializeField] private bool isInstantiated = false;
    [SerializeField] private string pokemonName = "";
    [SerializeField] private int pokedexIndex = 0;
    [SerializeField] private string pokemonCategory = "";
    [SerializeField] private Type[] types = new Type[1];
    [SerializeField] private Ability ability = null;
    [SerializeField] private HoldableItem itemInHand = null;
    [SerializeField, TextArea] private string description = "";

    [Header("Conditions:")]
    [SerializeField] private ConditionOversight oversight = null;

    [Header("Stats:")]
    [SerializeField] private float currentHealth = 0;
    [SerializeField] private int[] stats = new int[6];
    [SerializeField] private int[] iv = new int[6];
    [SerializeField] private int[] ev = new int[6];
    [SerializeField] private int level = 0, maxExp = 0;
    [SerializeField] private int currentExp = 0;

    [Header("Evolotion:")]
    [SerializeField] private EvolutionMethod method = 0;
    [SerializeField] Pokemon evolveTo = null;
    [SerializeField] int evolutionLevel = 0;

    [Header("Moves:")]
    [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];
    [Header(" -- Learnable:")]
    //Level
    [SerializeField] private int[] levelLearnableMoveKeys = new int[0];
    [SerializeField] private PokemonMove[] levelLearnableMoveValue = new PokemonMove[0];
    //TM/TR
    [SerializeField] private PokemonMove[] tmLearnableMoveValue = new PokemonMove[0];
    //Breeding
    [SerializeField] private Pokemon[] breedingLearnedMoveKeys = new Pokemon[0];
    [SerializeField] private PokemonMove[] breedingLearnedMoveValue = new PokemonMove[0];
    //Tutor
    [SerializeField] private PokemonMove[] tutorLearnableMoveValue = new PokemonMove[0];

    [Header("Breeding:")]
    [SerializeField] private EggGroup eggGroup = 0;
    [SerializeField] private int hatchTimeMin = 0, hatchTimeMax = 0;
    [SerializeField] private float height = 0, weight = 0;
    [SerializeField] private float genderRate = 0, catchRate = 0;

    [Header("Mich")]
    // ---Mega
    [SerializeField] private int expYield = 0;
    [SerializeField] private LevelRate levelRate = 0;
    [SerializeField] private int[] evYield = new int[6];
    [SerializeField] private Shape shape = 0;
    [SerializeField] private Footprint footprint = 0;
    [SerializeField] private Color pokedexColor = Color.green;
    [SerializeField] private int baseFriendship = 0;
    #endregion

    #region In Battle
    [Header("Battlefield:")]
    [SerializeField] private int spotIndex = 0;
    [Header("Visual:")]
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private GameObject spawnedObject = null;
    [Header(" -- Animation:")]
    [SerializeField] private Animator anim = null;
    [Header("Turn")]
    [SerializeField] private bool stunned = false, gettingSwitched = false;
    [Header("Action:")]
    [SerializeField] private BattleAction battleAction = null;
    #endregion
    #endregion

    #region Getters
    #region - Pokemon
    public Pokemon GetPokemon()
    {
        Pokemon result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(result);
            result.SetIsInstantiated(true);
        }

        return result;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public string GetName()
    {
        return pokemonName;
    }

    public int GetStatRaw(Stat target)
    {
        return stats[(int)target];
    }

    public int GetIV(Stat target)
    {
        return iv[(int)target];
    }

    public int GetEV(Stat target)
    {
        return ev[(int)target];
    }

    public int GetLevel()
    {
        return level;
    }

    public Type[] GetTypes()
    {
        return types;
    }

    public EggGroup GetEggGroup()
    {
        return eggGroup;
    }

    public int GetMinHatchSteps()
    {
        return hatchTimeMin;
    }

    public int GetMaxHatchSteps()
    {
        return hatchTimeMax;
    }

    public float GetHeight()
    {
        return height;
    }

    public float GetWeight()
    {
        return weight;
    }

    public int GetPokedexIndex()
    {
        return pokedexIndex;
    }

    public string GetPokemonCategory()
    {
        return pokemonCategory;
    }

    public int GetEVYield(Stat target)
    {
        return evYield[(int)target];
    }

    public int GetExpYield()
    {
        return expYield;
    }

    public LevelRate GetLevelRate()
    {
        return levelRate;
    }

    public float GetGenderRate()
    {
        return genderRate;
    }

    public float GetCatchRate()
    {
        return catchRate;
    }

    public int[] GetLevelLearnedMovesKeys()
    {
        return levelLearnableMoveKeys;
    }

    public PokemonMove[] GetLevelLearnableMoveValue()
    {
        return levelLearnableMoveValue;
    }

    public Pokemon[] GetBreedingLearnableMoveKeys()
    {
        return breedingLearnedMoveKeys;
    }

    public PokemonMove[] GetBreedingLearnableMoveValue()
    {
        return breedingLearnedMoveValue;
    }

    public PokemonMove[] GetTMLearnableMoveValues()
    {
        return tmLearnableMoveValue;
    }

    public PokemonMove[] GetTutorLearnableMoveValue()
    {
        return tutorLearnableMoveValue;
    }
    #endregion

    #region - In Battle
    public int GetStat(Stat stat)
    {
        int baseStat = stats[(int)stat];
        int baseIV = iv[(int)stat];
        int baseEV = ev[(int)stat];

        return BattleMathf.CalculateOtherStat(baseStat, baseIV, baseEV, level);
    }

    public ConditionOversight GetConditionOversight()
    {
        if (oversight == null)
        {
            oversight = CreateInstance("ConditionOversight") as ConditionOversight;
            oversight.SetIsInstantiated(true);
        }

        return oversight;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public PokemonMove[] GetMoves()
    {
        return learnedMoves;
    }

    public PokemonMove GetMoveByIndex(int index)
    {
        PokemonMove result = learnedMoves[index];
        if (result != null)
            return learnedMoves[index].GetAction() as PokemonMove;

        return null;
    }

    public int GetSpotIndex()
    {
        return spotIndex;
    }

    public GameObject GetPokemonPrefab()
    {
        return prefab;
    }

    public GameObject GetSpawnedObject()
    {
        return spawnedObject;
    }

    public BattleAction GetBattleAction()
    {
        return battleAction;
    }

    public Ability GetAbility()
    {
        return ability;
    }
    #endregion
    #endregion

    #region Setters
    #region Pokemon
    public void SetAbility(Ability toSet)
    {
        ability = toSet;
    }

    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }

    public void SetCurrentHP(int set)
    {
        currentHealth = set;
    }

    public void SetStat(Stat targetStat, int set)
    {
        stats[(int)targetStat] = set;
    }

    public void SetIV(Stat targetStat, int set)
    {
        iv[(int)targetStat] = set;
    }

    public void SetEV(Stat targetStat, int set)
    {
        ev[(int)targetStat] = set;
    }

    public void SetEggGroup(EggGroup egg)
    {
        eggGroup = egg;
    }

    public void SetMinHatchSteps(int set)
    {
        hatchTimeMin = set;
    }

    public void SetMaxHatchSteps(int set)
    {
        hatchTimeMax = set;
    }

    public void SetHeight(float set)
    {
        height = set;
    }

    public void SetWeight(float set)
    {
        weight = set;
    }

    public void SetLearnedMove(int moveIndex, PokemonMove move)
    {
        learnedMoves[moveIndex] = move;
    }

    public void SetTypes(Type[] set)
    {
        types = set;
    }

    public void SetPokedexIndex(int set)
    {
        pokedexIndex = set;
    }

    public void SetPokemonCategory(string set)
    {
        pokemonCategory = set;
    }

    public void SetEVYield(Stat target, int set)
    {
        evYield[(int)target] = set;
    }

    public void SetExpYield(int set)
    {
        expYield = set;
    }

    public void SetLevelingRate(LevelRate set)
    {
        levelRate = set;
    }

    public void SetGenderRate(float set)
    {
        genderRate = set;
    }

    public void SetCatchRate(float set)
    {
        catchRate = set;
    }

    public void SetLevelLearnedMoveKeys(int[] set)
    {
        levelLearnableMoveKeys = set;
    }

    public void SetLevelLearnableMoveValues(PokemonMove[] set)
    {
        levelLearnableMoveValue = set;
    }

    public void SetBreedingLearnableMoveKeys(Pokemon[] set)
    {
        breedingLearnedMoveKeys = set;
    }

    public void SetBreedingLearnableMoveValues(PokemonMove[] set)
    {
        breedingLearnedMoveValue = set;
    }

    public void SetTMLearnableMoveValues(PokemonMove[] set)
    {
        tmLearnableMoveValue = set;
    }

    public void SetTutorLearnableMoveValue(PokemonMove[] set)
    {
        tutorLearnableMoveValue = set;
    }
    #endregion

    #region Battle
    public void SetSpotIndex(int set)
    {
        spotIndex = set;
    }

    public void SetSpawnedObject(GameObject set)
    {
        spawnedObject = set;
    }

    public void SetBattleAction(BattleAction set)
    {
        battleAction = set;
    }
    #endregion
    #endregion

    #region Out
    public void DespawnPokemon()
    {
        Destroy(spawnedObject);
        spawnedObject = null;
    }

    public bool IsSameType(TypeName typeName)
    {
        if (types[0].GetTypeName() == typeName)
            return true;

        if (types[1] != null)
        {
            if (types[1].GetTypeName() == typeName)
                return true;
        }

        return false;
    }
    #endregion

    #region In
    public void RecieveDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, GetStat(Stat.HP));
    }

    public void RecieveExp(int points)
    {
        if (level != 100)
        {
            int expNeeded = maxExp - currentExp;

            if (expNeeded <= points)
            {

            }
            else
            {
                points -= expNeeded;

                LevelUp();

                currentExp = points;
            }
        }
    }
    #endregion

    #region Internal
    private void LevelUp()
    {
        level++;
    }
    #endregion
}