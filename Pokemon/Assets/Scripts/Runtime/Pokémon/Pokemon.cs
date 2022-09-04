#region Packages

using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Items;
using Runtime.Pokémon.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Pokémon
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

        [BoxGroup("Basic")] [SerializeField] private string pokemonName;

        [BoxGroup("Basic")] [SerializeField] private int pokedexIndex;

        [BoxGroup("Basic")] [SerializeField] private Type[] types = new Type[1];

        [BoxGroup("Basic")] [SerializeField, TextArea]
        private string description;

        [HorizontalGroup("Basic/H1"), VerticalGroup("Basic/H1/V1")] [SerializeField]
        private float height;

        [VerticalGroup("Basic/H1/V1")] [SerializeField]
        private float weight;

        [VerticalGroup("Basic/H1/V2")] [SerializeField]
        private Shape shape;

        [VerticalGroup("Basic/H1/V2")] [SerializeField]
        private Footprint footprint;

        [BoxGroup("Basic")] [SerializeField] private Color pokedexColor = Color.green;

        [HorizontalGroup("Basic/H2"), PreviewField(100), HideLabel, LabelWidth(0)] [SerializeField]
        private GameObject prefab;

        [BoxGroup("Moves")] [SerializeField] private string pokemonCategory;


        [VerticalGroup("Basic/H2/Abilities"), LabelWidth(100)] [SerializeField]
        private Ability firstAbility, secondAbility, hiddenAbility;


        [BoxGroup("Stats")] [SerializeField] private Stats stats;
        [BoxGroup("Stats")] [SerializeField] private int[] iv = new int[6];
        [BoxGroup("Stats")] [SerializeField] private int[] ev = new int[6];

        [BoxGroup("Evolution")] [SerializeField]
        private EvolutionMethod method;

        [BoxGroup("Evolution")] [SerializeField]
        Pokemon evolveTo;

        [BoxGroup("Evolution")] [SerializeField]
        int evolutionLevel;

        [BoxGroup("Moves")] [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];

        [BoxGroup("Moves")] [SerializeField] private int[] levelLearnableMoveKeys;

        [BoxGroup("Moves")] [SerializeField] private PokemonMove[] levelLearnableMove;

        [BoxGroup("Moves")] [SerializeField] private PokemonMove[] tmLearnableMove;

        [BoxGroup("Moves")] [SerializeField] private PokemonMove[] tutorLearnableMove;

        [BoxGroup("Breeding")] [SerializeField]
        private Pokemon[] breedingLearnedMoveKeys;

        [BoxGroup("Breeding")] [SerializeField]
        private PokemonMove[] breedingLearnedMove;

        [BoxGroup("Breeding")] [SerializeField]
        private EggGroup eggGroup;

        [BoxGroup("Breeding")] [SerializeField]
        private int hatchTimeMin, hatchTimeMax;

        private int maxHealth;

        private int level, maxExp;
        private int currentExp;

        private float currentHealth;

        private ConditionOversight oversight;

        private HoldableItem itemInHand;

        [SerializeField] private float genderRate, catchRate;

        [SerializeField] private int expYield;

        [SerializeField] private LevelRate levelRate;
        [SerializeField] private int[] evYield = new int[6];

        [SerializeField] private int baseFriendship;

        #endregion

        #region In Battle

        [Header("Battle:"), SerializeField] private int spotIndex;
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
            return stats[target];
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
            return levelLearnableMove;
        }

        public Pokemon[] GetBreedingLearnableMoveKeys()
        {
            return breedingLearnedMoveKeys;
        }

        public PokemonMove[] GetBreedingLearnableMoveValue()
        {
            return breedingLearnedMove;
        }

        public PokemonMove[] GetTMLearnableMoveValues()
        {
            return tmLearnableMove;
        }

        public PokemonMove[] GetTutorLearnableMoveValue()
        {
            return tutorLearnableMove;
        }

        public Ability[] GetAbilities()
        {
            return new[] { firstAbility, secondAbility, hiddenAbility };
        }

        #endregion

        #region - In Battle

        public int GetCalculatedStat(Stat stat)
        {
            int baseStat = this.stats[stat];

            int iv = this.iv[(int)stat];

            int ev = this.ev[(int)stat];

            float result = stat == Stat.HP
                ? BattleMathf.CalculateHPStat(
                    baseStat,
                    iv,
                    ev,
                    this.level)
                : BattleMathf.CalculateBaseStat(
                    baseStat,
                    iv,
                    ev,
                    this.level);

            result *= BattleMathf.GetMultiplierValue(this.multipliers[(int)stat],
                stat is not (Stat.Accuracy or Stat.Evasion));

            if (stat == Stat.HP) return Mathf.FloorToInt(result);

            result = BattleManager.instance.GetAbilityOversight()
                .ListOfSpecific<IStatModifier>()
                .Aggregate(result, (current, statModifier) => current * statModifier.Modify(this, stat));

            return Mathf.FloorToInt(result);
        }

        public ConditionOversight GetConditionOversight()
        {
            return this.oversight ? this.oversight : this.oversight = CreateInstance<ConditionOversight>();
        }

        public float GetCurrentHealth()
        {
            return this.currentHealth;
        }

        public IEnumerable<PokemonMove> GetMoves()
        {
            return this.learnedMoves;
        }

        public PokemonMove GetMoveByIndex(int index)
        {
            if (this.learnedMoves.Length <= 0 || index < 0 || index >= this.learnedMoves.Length) return null;

            if (this.learnedMoves[index] != null)
                return this.learnedMoves[index].GetAction() as PokemonMove;

            return null;
        }

        public GameObject GetPokemonPrefab()
        {
            return this.prefab;
        }

        public GameObject GetSpawnedObject()
        {
            return this.spawnedObject;
        }

        public BattleAction GetBattleAction()
        {
            return this.battleAction;
        }

        public bool GetGettingSwitched()
        {
            return this.gettingSwitched;
        }

        public bool GetInBattle()
        {
            return this.inBattle;
        }

        public bool GetRevived()
        {
            return this.gettingRevived;
        }

        public int GetAccuracy()
        {
            return this.accuracy;
        }

        public int GetEvasion()
        {
            return this.evasion;
        }

        public int GetCritical()
        {
            return this.critical;
        }

        #endregion

        #endregion

        #region Setters

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

        #region Out

        public bool IsSameType(TypeName typeName)
        {
            if (this.types[0].GetTypeName() == typeName)
                return true;

            if (this.types[1] == null) return false;

            return this.types[1].GetTypeName() == typeName;
        }

        public T[] GetAbilitiesOfType<T>()
        {
            return new[] { this.firstAbility, this.secondAbility, this.hiddenAbility }.OfType<T>().ToArray();
        }

        public PokemonBattleInstance CreateBattleInstance()
        {
            return new PokemonBattleInstance(this);
        }

        #endregion

        #region In

        public void Setup()
        {
            this.oversight ??= CreateInstance<ConditionOversight>();
            this.oversight.Setup(this);

            this.maxHealth = GetCalculatedStat(Stat.HP);

            AbilityOversight abilityOversight = BattleManager.instance.GetAbilityOversight();

            this.instantiatedAbilities = new List<Ability> { firstAbility, secondAbility, hiddenAbility };

            for (int i = 0; i < this.instantiatedAbilities.Count; i++)
            {
                if (this.instantiatedAbilities[i] is null) continue;

                Ability ability = Instantiate(this.instantiatedAbilities[i]);

                ability.SetAffectedPokemon(this);
                abilityOversight.AddAbility(ability);

                this.instantiatedAbilities[i] = ability;
            }
        }

        // ReSharper disable once IdentifierTypo
        public void DespawnPokemon()
        {
            Destroy(this.spawnedObject);
            this.inBattle = false;
            this.gettingSwitched = false;
            this.spawnedObject = null;
            this.oversight.ResetConditionList();
        }

        public void ReceiveDamage(float damage)
        {
            this.currentHealth = Mathf.Clamp(this.currentHealth - damage, 0, this.maxHealth);
        }

        public void ReceiveExp(int points)
        {
            if (this.level == 100) return;

            int expNeeded = this.maxExp - this.currentExp;

            if (expNeeded < points)
            {
            }
            else
            {
                points -= expNeeded;
                LevelUp();
                this.currentExp = points;
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