#region SDK

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

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
        Speed
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
        #region Values:

        #region Pokemon

        [Header("Object Reference:")] [SerializeField]
        private bool isInstantiated;

        [SerializeField] private string pokemonName;

        // ReSharper disable once IdentifierTypo
        [SerializeField] private int pokedexIndex;
        [SerializeField] private string pokemonCategory;
        [SerializeField] private Type[] types = new Type[1];
        [SerializeField] private Ability ability;
        [SerializeField] private HoldableItem itemInHand;
        [SerializeField, TextArea] private string description;

        [Header("Conditions:")] [SerializeField]
        private ConditionOversight oversight;

        [Header("Stats:")] [SerializeField] private float currentHealth;
        [SerializeField] private int[] stats = new int[6];
        [SerializeField] private int[] iv = new int[6];
        [SerializeField] private int[] ev = new int[6];
        [SerializeField] private int level, maxExp;
        [SerializeField] private int currentExp;

        [Header("Evolution:")] [SerializeField]
        private EvolutionMethod method;

        [SerializeField] Pokemon evolveTo;
        [SerializeField] int evolutionLevel;

        [Header("Moves:")] [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];

        [Header(" -- Learnable:")]
        //Level
        [SerializeField]
        private int[] levelLearnableMoveKeys = new int[0];

        [SerializeField] private PokemonMove[] levelLearnableMoveValue = new PokemonMove[0];

        //TM/TR
        [SerializeField] private PokemonMove[] tmLearnableMoveValue = new PokemonMove[0];

        //Breeding
        [SerializeField] private Pokemon[] breedingLearnedMoveKeys = new Pokemon[0];

        [SerializeField] private PokemonMove[] breedingLearnedMoveValue = new PokemonMove[0];

        //Tutor
        [SerializeField] private PokemonMove[] tutorLearnableMoveValue = new PokemonMove[0];

        [Header("Breeding:")] [SerializeField] private EggGroup eggGroup;
        [SerializeField] private int hatchTimeMin, hatchTimeMax;
        [SerializeField] private float height, weight;
        [SerializeField] private float genderRate, catchRate;

        [Header("Miscellaneous")] [SerializeField]
        private int expYield;

        [SerializeField] private LevelRate levelRate;
        [SerializeField] private int[] evYield = new int[6];
        [SerializeField] private Shape shape;
        [SerializeField] private Footprint footprint;
        // ReSharper disable once IdentifierTypo
        [SerializeField] private Color pokedexColor = Color.green;
        [SerializeField] private int baseFriendship;

        #endregion

        #region In Battle

        [Header("Battle:")] [SerializeField] private int spotIndex = 0;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private GameObject spawnedObject = null;
        [SerializeField] private Animator anim = null;
        [SerializeField] private bool gettingSwitched = false, inBattle = false;
        [SerializeField] private BattleAction battleAction = null;
        [SerializeField] private bool gettingRevived = false;

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
            return stats[(int) target];
        }

        // ReSharper disable once InconsistentNaming
        public int GetIV(Stat target)
        {
            return iv[(int) target];
        }

        // ReSharper disable once InconsistentNaming
        public int GetEV(Stat target)
        {
            return ev[(int) target];
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
            return evYield[(int) target];
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
            int baseStat = stats[(int) stat];
            // ReSharper disable once InconsistentNaming
            int baseIV = iv[(int) stat];
            // ReSharper disable once InconsistentNaming
            int baseEV = ev[(int) stat];

            return BattleMathf.CalculateOtherStat(baseStat, baseIV, baseEV, level);
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
            PokemonMove result = null;
            if (learnedMoves.Length > 0 && index >= 0 && index < learnedMoves.Length)
            {
                if (learnedMoves[index] != null)
                    result = (learnedMoves[index].GetAction() as PokemonMove);
            }

            return result;
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

        #endregion

        #endregion

        #region Setters

        #region Pokemon

        public void SetAbility(Ability toSet)
        {
            ability = toSet;
        }

        public void SetStat(Stat targetStat, int set)
        {
            stats[(int) targetStat] = set;
        }

        // ReSharper disable once InconsistentNaming
        public void SetIV(Stat targetStat, int set)
        {
            iv[(int) targetStat] = set;
        }

        // ReSharper disable once InconsistentNaming
        public void SetEV(Stat targetStat, int set)
        {
            ev[(int) targetStat] = set;
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
            evYield[(int) target] = set;
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

        #endregion

        #region In

        public void DespawnPokemon()
        {
            Destroy(spawnedObject);

            inBattle = false;
            gettingSwitched = false;
            spawnedObject = null;

            oversight.ResetConditionList();
        }

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

        private void SetIsInstantiated(bool set)
        {
            isInstantiated = set;
        }

        private void LevelUp()
        {
            level++;
        }

        #endregion
    }
}