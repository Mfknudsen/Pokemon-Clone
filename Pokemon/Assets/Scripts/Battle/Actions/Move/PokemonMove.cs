#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;
using Random = UnityEngine.Random;
using Type = Mfknudsen.Pokémon.Type;

#endregion

namespace Mfknudsen.Battle.Actions.Move
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
        AllExceptUser,
        All,
    }

    #endregion

    [CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new Move", order = 1)]
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

        [Header("Interaction:")] [SerializeField]
        protected bool makesContact;

        [SerializeField] protected bool affectByProtect,
            affectByMagicCoat,
            affectBySnatch,
            affectByMirrorMove,
            affectByKingsRock;

        [Header("Target:")] [SerializeField] protected HitType hitType = 0;
        [SerializeField] protected bool selfTargetable;
        [SerializeField] protected bool[] enemyTargetable = new bool[3];
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
        [SerializeField] protected float applyChange;
        [SerializeField] protected bool statusHit;
        [SerializeField] Chat statusHitChat, statusFailedChat;

        #region Operation

        private Spot currentSpot;

        private float damagePerTarget,
            damageApplied,
            damageOverTime;

        #endregion

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

        public void SetApplyChange(float set)
        {
            applyChange = set;
        }

        public void SetMaxPP(int set)
        {
            maxPP = set;
        }

        #endregion

        #region Getters

        public override BattleAction GetAction()
        {
            BattleAction result = this;

            if (!result.GetIsInstantiated())
            {
                result = Instantiate(this);
                result.SetIsInstantiated(true);
            }

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
            return new int[] {(int) normalCondition, normalAppeal, normalJam};
        }

        public int[] GetSuperContests()
        {
            return new int[] {(int) superCondition, superAppeal, superJam};
        }

        public int[] GetSpectacularContests()
        {
            return new int[] {(int) spectacularCondition, spectacularAppeal, spectacularJam};
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

        public float GetApplyChange()
        {
            return applyChange;
        }

        public int GetMaxPP()
        {
            return maxPP;
        }

        #endregion

        #region Out

        public override IEnumerator Activate()
        {
            if (!active)
            {
                ChatMaster.instance.Add(TransferInformationToChat());

                active = true;
                done = false;
            }

            if (category != Category.Status)
            {
                if (targetPokemon.Count > 0)
                {
                    if (category == Category.Physical || category == Category.Special)
                    {
                        float attack = currentPokemon.GetStat(Stat.SpAtk);

                        if (category == Category.Physical)
                            attack = currentPokemon.GetStat(Stat.Attack);

                        damagePerTarget = new float[targetPokemon.Count];


                        for (int i = 0; i < targetPokemon.Count; i++)
                        {
                            Pokemon p = targetPokemon[i];
                            float defence = p.GetStat(Stat.SpDef);

                            if (category == Category.Physical)
                                defence = p.GetStat(Stat.Defence);

                            int damage = BattleMathf.CalculateDamage(
                                currentPokemon.GetLevel(),
                                attack,
                                defence,
                                power,
                                BattleMathf.CalculateModifiers(
                                    currentPokemon, p, this, (targetPokemon.Count == 1)));

                            damagePerTarget[i] = damage;
                        }
                    }
                }
                else
                    Debug.Log("No targets to hit");
            }
            else
            {
                if (statusCondition != null)
                {
                    if (Random.Range(0.0f, 1.0f) <= (applyChange / 100))
                    {
                        foreach (Pokemon target in targetPokemon)
                        {
                            Condition toApply = statusCondition.GetCondition();
                            toApply.SetAffectedPokemon(target);

                            if (target.GetConditionOversight().TryApplyNonVolatileCondition(toApply))
                            {
                                statusHit = true;
                                if (statusHitChat != null)
                                {
                                    Chat toSend = statusHitChat.GetChat();
                                    toSend.AddToOverride("<TARGET_NAME>", target.GetName());
                                    ChatMaster.instance.Add(toSend);
                                }
                            }
                            else if (statusFailedChat != null)
                            {
                                Chat toSend = statusFailedChat.GetChat();
                                toSend.AddToOverride("<TARGET_NAME>", target.GetName());
                                toSend.AddToOverride("<CONDITION_EFFECT>", toApply.GetConditionEffect());
                                ChatMaster.instance.Add(toSend);
                            }
                        }
                    }
                    else
                        Debug.Log("Temp Miss");
                }
            }

            return Operation();
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

        #endregion

        #region IEnumerator

        protected override IEnumerator Operation()
        {
            if (category != Category.Status)
            {
                if (damagePerTarget.Length > 0)
                {
                    float divide = 200 * BattleMaster.instance.GetSecPerPokeMove();

                    float[] damageApplied = new float[targetPokemon.Count];
                    float[] damageOverTime = new float[targetPokemon.Count];

                    for (int i = 0; i < targetPokemon.Count; i++)
                        damageOverTime[i] = damagePerTarget[i] / divide;

                    while (damageApplied[0] < damagePerTarget[0])
                    {
                        for (int i = 0; i < targetPokemon.Count; i++)
                        {
                            if (Mathf.Clamp(damageApplied[i] + damageOverTime[i], 0, damagePerTarget[i]) ==
                                damagePerTarget[i])
                                damageOverTime[i] = damagePerTarget[i] - damageApplied[i];

                            damageApplied[i] = Mathf.Clamp(damageApplied[i] + damageOverTime[i], 0, damagePerTarget[i]);

                            targetPokemon[i].RecieveDamage(damageOverTime[i]);
                        }

                        if (damageApplied[0] == damagePerTarget[0])
                            yield return null;
                        else
                            yield return new WaitForSeconds(BattleMaster.instance.GetSecPerPokeMove() / divide);
                    }
                }
            }
            else if (statusHit)
            {
                yield return new WaitForSeconds(0.75f);
            }

            done = true;

            yield return null;
        }

        #endregion
    }
}