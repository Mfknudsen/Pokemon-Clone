#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Items;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

// ReSharper disable InconsistentNaming
namespace Mfknudsen.Pokémon
{
    #region Enums

    public enum Stat
    {
        // ReSharper disable once InconsistentNaming
        HP,
        Attack,
        Defence,
        SpAtk,
        SpDef,
        Speed,
        Accuracy,
        Evasion,
        Critical
    }

    public enum EvolutionMethod
    {
        None,
        Level,
        Item,
        Trade,
        LearnedMove
    }

    public enum EggGroup
    {
        Monster,
        Water1
    }

    public enum LevelRate
    {
        Slow,
        MediumSlow
    }

    public enum Shape
    {
    }

    public enum Footprint
    {
    }

    #endregion

    [CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon", order = 0)]
    public class Pokemon : ScriptableObject
    {
        #region Values

        #region Pokemon

        [SerializeField] private bool isInstantiated;

        [SerializeField] private string pokemonName;

        // ReSharper disable once IdentifierTypo
        [SerializeField] private int pokedexIndex;
        [SerializeField] private string pokemonCategory;
        [SerializeField] private Type[] types = new Type[1];
        [SerializeField] private Ability firstAbility, secondAbility, hiddenAbility;
        [SerializeField] private HoldableItem itemInHand;
        [SerializeField, TextArea] private string description;

        [SerializeField] private ConditionOversight oversight;

        private int maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private int[] stats = new int[6];
        [SerializeField] private int[] iv = new int[6];
        [SerializeField] private int[] ev = new int[6];
        [SerializeField] private int level, maxExp;
        [SerializeField] private int currentExp;

        [SerializeField] private EvolutionMethod method;

        [SerializeField] Pokemon evolveTo;
        [SerializeField] int evolutionLevel;
        [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];

        //Level
        [SerializeField] private int[] levelLearnableMoveKeys;

        [SerializeField] private PokemonMove[] levelLearnableMoveValue;

        //TM/TR
        [SerializeField] private PokemonMove[] tmLearnableMoveValue;

        //Breeding
        [SerializeField] private Pokemon[] breedingLearnedMoveKeys;

        [SerializeField] private PokemonMove[] breedingLearnedMoveValue;

        //Tutor
        [SerializeField] private PokemonMove[] tutorLearnableMoveValue;

        [SerializeField] private EggGroup eggGroup;
        [SerializeField] private int hatchTimeMin, hatchTimeMax;
        [SerializeField] private float height, weight;
        [SerializeField] private float genderRate, catchRate;

        [SerializeField] private int expYield;

        [SerializeField] private LevelRate levelRate;
        [SerializeField] private int[] evYield = new int[6];
        [SerializeField] private Shape shape;
        [SerializeField] private Footprint footprint;

        // ReSharper disable once IdentifierTypo
        [SerializeField] private Color pokedexColor = Color.green;
        [SerializeField] private int baseFriendship;

        #endregion

        #region In Battle

        [Header("Battle:"), SerializeField] private int spotIndex;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject spawnedObject;
        [SerializeField] private Animator anim;
        [SerializeField] private bool gettingSwitched, inBattle;
        [SerializeField] private BattleAction battleAction;
        [SerializeField] private bool gettingRevived;
        [SerializeField] private int[] multipliers = new int[6];
        [SerializeField] private List<Ability> instantiatedAbilities;

        private int accuracy = 0, evasion = 0, critical = 0;

        #endregion

        #endregion

        #region Getters

        #region Pokemon

        public int GetMaxHealth()
        {
            return maxHealth;
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

        // ReSharper disable once InconsistentNaming
        public int GetIV(Stat target)
        {
            return iv[(int)target];
        }

        // ReSharper disable once InconsistentNaming
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
            if (types.Length == 1)
                return (Type[])types.Concat(new Type[] { null });

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

        // ReSharper disable once IdentifierTypo
        public int GetPokedexIndex()
        {
            return pokedexIndex;
        }

        public string GetPokemonCategory()
        {
            return pokemonCategory;
        }

        // ReSharper disable once InconsistentNaming
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

        public Ability[] GetAbilities()
        {
            return new[] { firstAbility, secondAbility, hiddenAbility };
        }

        #endregion

        #region - In Battle

        public int GetStat(Stat stat)
        {
            int baseStat = stats[(int)stat];

            int IV = iv[(int)stat];

            int EV = ev[(int)stat];

            float result = stat == Stat.HP
                ? BattleMathf.CalculateHPStat(
                    baseStat,
                    IV,
                    EV,
                    level)
                : BattleMathf.CalculateBaseStat(
                    baseStat,
                    IV,
                    EV,
                    level);

            result *= BattleMathf.GetMultiplierValue(multipliers[(int)stat],
                !(stat == Stat.Accuracy || stat == Stat.Evasion));

            if (stat == Stat.HP) return Mathf.FloorToInt(result);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (IStatModifier statModifier in BattleManager.instance.GetAbilityOversight()
                .ListOfSpecific<IStatModifier>())
            {
                result *= statModifier.Modify(this, stat);
            }

            return Mathf.FloorToInt(result);
        }

        public ConditionOversight GetConditionOversight()
        {
            return oversight ? oversight : oversight = CreateInstance<ConditionOversight>();
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
            if (learnedMoves.Length <= 0 || index < 0 || index >= learnedMoves.Length) return null;

            if (learnedMoves[index] != null)
                return learnedMoves[index].GetAction() as PokemonMove;

            return null;
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

        public bool GetGettingSwitched()
        {
            return gettingSwitched;
        }

        public bool GetInBattle()
        {
            return inBattle;
        }

        public bool GetRevived()
        {
            return gettingRevived;
        }

        public int GetAccuracy()
        {
            return accuracy;
        }

        public int GetEvasion()
        {
            return evasion;
        }

        public int GetCritical()
        {
            return critical;
        }

        #endregion

        #endregion

        #region Setters

        #region Pokemon

        public void SetStat(Stat targetStat, int set)
        {
            stats[(int)targetStat] = set;
        }

        // ReSharper disable once InconsistentNaming
        public void SetIV(Stat targetStat, int set)
        {
            iv[(int)targetStat] = set;
        }

        // ReSharper disable once InconsistentNaming
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

        // ReSharper disable once InconsistentNaming
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

        public void SetIsInstantiated(bool set)
        {
            isInstantiated = set;
        }

        public void SetSpawnedObject(GameObject set)
        {
            spawnedObject = set;
        }

        public void SetBattleAction(BattleAction set)
        {
            battleAction = set;
        }

        public void SetGettingSwitched(bool set)
        {
            gettingSwitched = set;
        }

        public void SetInBattle(bool set)
        {
            inBattle = set;
        }

        public void SetRevived(bool set)
        {
            gettingRevived = set;
        }

        public void SetFirstAbility(Ability set)
        {
            firstAbility = set;
        }

        public void SetSecondAbility(Ability set)
        {
            secondAbility = set;
        }

        public void SetHiddenAbility(Ability set)
        {
            hiddenAbility = set;
        }

        #endregion

        #endregion

        #region Out

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

        public T[] GetAbilitiesOfType<T>()
        {
            return new[] { firstAbility, secondAbility, hiddenAbility }.OfType<T>().ToArray();
        }

        #endregion

        #region In

        public void Setup()
        {
            oversight ??= CreateInstance<ConditionOversight>();
            oversight.Setup(this);

            maxHealth = GetStat(Stat.HP);

            AbilityOversight abilityOversight = BattleManager.instance.GetAbilityOversight();

            instantiatedAbilities = new List<Ability> { firstAbility, secondAbility, hiddenAbility };

            for (int i = 0; i < instantiatedAbilities.Count; i++)
            {
                if (instantiatedAbilities[i] is null) continue;

                Ability ability = Instantiate(instantiatedAbilities[i]);

                ability.SetAffectedPokemon(this);
                abilityOversight.AddAbility(ability);

                instantiatedAbilities[i] = ability;
            }
        }

        // ReSharper disable once IdentifierTypo
        public void DespawnPokemon()
        {
            Destroy(spawnedObject);
            inBattle = false;
            gettingSwitched = false;
            spawnedObject = null;
            oversight.ResetConditionList();
        }

        public void ReceiveDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        }

        public void ReceiveExp(int points)
        {
            if (level == 100) return;

            int expNeeded = maxExp - currentExp;

            if (expNeeded < points)
            {
            }
            else
            {
                points -= expNeeded;
                LevelUp();
                currentExp = points;
            }
        }

        public void EffectMultiplierStage(int stages, Stat affectedStat)
        {
            if (affectedStat == Stat.Accuracy || affectedStat == Stat.Evasion || affectedStat == Stat.Critical)
                return;

            multipliers[(int)affectedStat] += stages;
        }

        public void AffectAccuracy(int affect)
        {
            accuracy = Mathf.Clamp(accuracy + affect, 0, 6);
        }

        public void AffectEvasion(int affect)
        {
            evasion = Mathf.Clamp(evasion + affect, 0, 6);
        }

        public void AffectCritical(int affect)
        {
            critical = Mathf.Clamp(critical + affect, 0, 6);
        }

        public void ResetForAIMemory()
        {
            for (int i = 0; i < 6; i++)
            {
                iv[i] = 0;
                ev[i] = 0;
            }

            firstAbility = null;
            secondAbility = null;
            hiddenAbility = null;

            baseFriendship = 0;

            oversight = Instantiate(oversight);
            oversight.Setup(this);
        }

        #endregion

        #region Internal

        private void LevelUp()
        {
            level++;
        }

        #endregion
    }
}