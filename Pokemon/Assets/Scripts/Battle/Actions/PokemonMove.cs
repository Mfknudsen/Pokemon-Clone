#region Packages

using System.Collections;
using Mfknudsen.AI;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Static_Operations;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;
using Type = Mfknudsen.Pokémon.Type;

#endregion

// ReSharper disable InconsistentNaming
namespace Mfknudsen.Battle.Actions
{
    #region Enums

    public enum Category
    {
        Physical,
        Special,
        Status,
        Extents
    }

    public enum Contest
    {
        Tough,
        Cute,
        Clever,
        Beautiful,
        Cool,
    }

    public enum HitType
    {
        One,
        AllAdjacentOneSide,
        AllAdjacent,
        AllOneSide,
        AllOneSideExceptUser,
        AllExceptUser,
        All,
    }

    public enum SpecialAddons
    {
        Sound,
        Bite
    }

    #endregion

    [CreateAssetMenu(fileName = "New Basic Pokemon Move", menuName = "Pokemon/Create new Basic Move", order = 1)]
    public class PokemonMove : BattleAction
    {
        #region Values

        [Header("Pokemon Move:")] [SerializeField]
        protected int index;

        [SerializeField] protected string moveName = "";
        [SerializeField] protected Type type;
        [SerializeField] protected Category category = 0;
        [SerializeField] protected int startPP, maxPP;
        private int currentPP;
        [SerializeField] protected int power, accuracy;

        // ReSharper disable once IdentifierTypo
        [SerializeField] private bool canCrit = true;

        [Header("Interaction:")] [SerializeField]
        protected bool makesContact;

        [SerializeField] protected bool affectByProtect,
            affectByMagicCoat,
            affectBySnatch,
            affectByMirrorMove,
            affectByKingsRock;

        [SerializeField] protected SpecialAddons[] specialAddons;

        [Header("Target:")] [SerializeField] protected HitType hitType = 0;

        // ReSharper disable once IdentifierTypo
        [SerializeField] protected bool selfTargetable;

        // ReSharper disable once IdentifierTypo
        [SerializeField] protected bool[] enemyTargetable = new bool[3];

        // ReSharper disable once IdentifierTypo
        [SerializeField] protected bool[] allyTargetable = new bool[2];

        [Header("Contests:")]
        //Standard
        [SerializeField]
        protected Contest normalCondition = 0;

        [SerializeField] protected int normalAppeal, normalJam;

        //Super
        [SerializeField] protected Contest superCondition = 0;

        [SerializeField] protected int superAppeal, superJam;

        //Spectacular
        [SerializeField] protected Contest spectacularCondition = 0;
        [SerializeField] protected int spectacularAppeal, spectacularJam;

        [Header("Status:")] [SerializeField] protected bool hasStatus;
        [SerializeField] protected Condition statusCondition;
        [SerializeField] protected int applyChance;
        [SerializeField] private Chat statusHitChat;

        private void OnValidate()
        {
            if (startPP < 0)
            {
                startPP = 0;
                Debug.LogError("Max PP of " + moveName + " should be more then 0!");
            }

            currentPP = startPP;
        }

        #endregion

        #region Setters

        public void SetType(Type set)
        {
            type = set;
        }

        public void SetCategory(Category set)
        {
            category = set;
        }

        // ReSharper disable once InconsistentNaming
        public void SetStartPP(int set)
        {
            startPP = set;
        }

        public void SetPower(int set)
        {
            power = set;
        }

        public void SetAccuracy(int set)
        {
            accuracy = set;
        }

        // ReSharper disable once IdentifierTypo
        public void SetTargetable(bool[] set)
        {
            enemyTargetable[0] = set[0];
            enemyTargetable[1] = set[1];
            enemyTargetable[2] = set[2];

            allyTargetable[0] = set[3];
            allyTargetable[1] = set[4];

            selfTargetable = set[5];
        }

        public void SetAffected(bool[] set)
        {
            makesContact = set[0];
            affectByProtect = set[1];
            affectByMagicCoat = set[2];
            affectBySnatch = set[3];
            affectByMirrorMove = set[4];
            affectByKingsRock = set[5];
        }

        public void SetNormalContests(int[] set)
        {
            normalCondition = (Contest) set[0];
            normalAppeal = set[1];
            normalJam = set[2];
        }

        public void SetSuperContests(int[] set)
        {
            superCondition = (Contest) set[0];
            superAppeal = set[1];
            superJam = set[2];
        }

        public void SetSpectacularContests(int[] set)
        {
            spectacularCondition = (Contest) set[0];
            spectacularAppeal = set[1];
            spectacularJam = set[2];
        }

        public void SetHitType(HitType set)
        {
            hitType = set;
        }

        public void SetHasStatus(bool set)
        {
            hasStatus = set;
        }

        public void SetStatusCondition(Condition set)
        {
            statusCondition = set;
        }

        public void SetApplyChance(int set)
        {
            applyChance = set;
        }

        // ReSharper disable once InconsistentNaming
        public void SetMaxPP(int set)
        {
            maxPP = set;
        }

        public void SetSpecialAddons(SpecialAddons[] set)
        {
            specialAddons = set;
        }

        #endregion

        #region Getters

        public override BattleAction GetAction()
        {
            BattleAction result = this;

            if (result.GetIsInstantiated()) return result;

            result = Instantiate(this);
            result.SetIsInstantiated(true);

            return result;
        }

        public string GetName()
        {
            return moveName;
        }

        public bool GetActive()
        {
            return active;
        }

        public Type GetMoveType()
        {
            return type;
        }

        public Category GetCategory()
        {
            return category;
        }

        // ReSharper disable once InconsistentNaming
        public int GetStartPP()
        {
            return startPP;
        }

        public int GetPower()
        {
            return power;
        }

        public int GetAccuracy()
        {
            return accuracy;
        }

        // ReSharper disable once IdentifierTypo
        public bool[] GetTargetable()
        {
            bool[] result = new bool[6];

            result[0] = enemyTargetable[0];
            result[1] = enemyTargetable[1];
            result[2] = enemyTargetable[2];

            result[3] = allyTargetable[0];
            result[4] = allyTargetable[1];

            result[5] = selfTargetable;

            return result;
        }

        public bool[] GetAffected()
        {
            bool[] result = new bool[6];

            result[0] = makesContact;
            result[1] = affectByProtect;
            result[2] = affectByMagicCoat;
            result[3] = affectBySnatch;
            result[4] = affectByMirrorMove;
            result[5] = affectByKingsRock;

            return result;
        }

        public int[] GetNormalContests()
        {
            return new[] {(int) normalCondition, normalAppeal, normalJam};
        }

        public int[] GetSuperContests()
        {
            return new[] {(int) superCondition, superAppeal, superJam};
        }

        public int[] GetSpectacularContests()
        {
            return new[] {(int) spectacularCondition, spectacularAppeal, spectacularJam};
        }

        public HitType GetHitType()
        {
            return hitType;
        }

        public bool GetHasStatus()
        {
            return hasStatus;
        }

        public Condition GetStatusCondition()
        {
            return statusCondition;
        }

        public int GetApplyChance()
        {
            return applyChance;
        }

        // ReSharper disable once InconsistentNaming
        public int GetMaxPP()
        {
            return maxPP;
        }

        public SpecialAddons[] GetSpecialAddons()
        {
            return specialAddons;
        }

        #endregion

        #region Out

        public override float Evaluate(Pokemon user, Pokemon target, VirtualBattle virtualBattle,
            PersonalitySetting personalitySetting)
        {
            return VirtualMathf.CalculateVirtualDamage(this, user, target, virtualBattle) *
                   personalitySetting.aggressionLevel;
        }

        #endregion

        #region Internal

        protected override Chat[] TransferInformationToChat()
        {
            Chat[] result = new Chat[chatOnActivation.Length];

            for (int i = 0; i < chatOnActivation.Length; i++)
            {
                if (chatOnActivation[i] == null) continue;

                result[i] = chatOnActivation[i].GetChat();

                result[i].AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                result[i].AddToOverride("<POKEMON_MOVE>", moveName);
            }

            return result;
        }

        private float GetDamageForTarget(Pokemon user, Pokemon target, bool isCritical)
        {
            float attackPower = currentPokemon.GetStat(category == Category.Physical ? Stat.Attack : Stat.SpAtk);

            float defencePower = currentPokemon.GetStat(category == Category.Physical ? Stat.Defence : Stat.SpDef);

            return BattleMathf.CalculateDamage(user.GetLevel(),
                attackPower,
                defencePower,
                power,
                BattleMathf.CalculateModifiers(
                    user,
                    target,
                    this,
                    targetPokemon.Count == 1,
                    isCritical));
        }

        #endregion

        #region IEnumerator

        public override IEnumerator Operation()
        {
            done = false;

            float secPerPokeMove = BattleManager.instance.GetSecPerPokeMove();
            OperationManager operationManager = OperationManager.Instance;


            foreach (Pokemon pokemon in targetPokemon)
            {
                OperationsContainer container = new OperationsContainer();

                #region Calculate Hit

                float accuracyUser = BattleMathf.CalculateOtherStat(
                    Stat.Accuracy,
                    currentPokemon.GetAccuracy());
                float evasionTarget = BattleMathf.CalculateOtherStat(
                    Stat.Evasion,
                    currentPokemon.GetEvasion());

                bool hit = BattleMathf.CalculateHit(accuracy, accuracyUser, evasionTarget);

                if (!hit)
                {
                    MissHit missHit = new MissHit(pokemon);
                    container.Add(missHit);
                    operationManager.AddOperationsContainer(container);

                    done = true;
                    yield return 1;
                    yield break;
                }

                #endregion

                foreach (Chat chat in chatOnActivation)
                {
                    Chat instance = chat.GetChat();
                    instance.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                    instance.AddToOverride("<POKEMON_MOVE>", moveName);

                    ChatManager.instance.Add(instance);
                }

                if (category != Category.Status)
                {
                    bool isCritical = canCrit && BattleMathf.CalculateCriticalRoll(currentPokemon, pokemon);
                    float damagePerTarget = GetDamageForTarget(currentPokemon, pokemon, isCritical);

                    DamagePokemon damagePokemon = new DamagePokemon(pokemon, damagePerTarget, secPerPokeMove);
                    container.Add(damagePokemon);
                }
                else if (!(statusCondition is null))
                {
                    if (!BattleMathf.CalculateStatusHit(applyChance)) continue;

                    ApplyStatus applyStatus = new ApplyStatus(statusHitChat, pokemon, statusCondition);
                    container.Add(applyStatus);
                }

                operationManager.AddOperationsContainer(container);
            }

            currentPokemon.SetBattleAction(null);
            done = true;
        }

        #endregion
    }
}