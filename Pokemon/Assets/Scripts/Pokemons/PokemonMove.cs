#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

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
    Beutiful,
    Cool,
}
public enum HitType
{
    One,
    AllAdjacentOneSide,
    AllAdjacent,
    AllOneSide,
    All,
}
#endregion

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new Move", order = 1)]
public class PokemonMove : BattleAction
{
    #region Values
    [Header("Pokemon Move:")]
    [SerializeField] protected int index = 0;
    [SerializeField] protected string moveName = "";
    [SerializeField] protected Type type = null;
    [SerializeField] protected Category category = 0;
    [SerializeField] protected int startPP = 0, maxPP = 0;
    protected int currentPP = 0;
    [SerializeField] protected int power = 0, accuracy = 0;

    [Header("Interaction:")]
    [SerializeField] protected bool makesContact = false;
    [SerializeField] protected bool affectByProtect = false, affectByMagicCoat = false, affectBySnatch = false, affectByMirrorMove = false, affectByKingsRock = false;

    [Header("Target:")]
    [SerializeField] protected int targetIndex = -1;
    [SerializeField] protected HitType hitType = 0;
    [SerializeField] protected bool selfTargetable = false;
    [SerializeField] protected bool[] enemyTargetable = new bool[3];
    [SerializeField] protected bool[] allyTargetable = new bool[2];

    [Header("Contests:")]
    //Standard
    [SerializeField] protected Contest normalCondition = 0;
    [SerializeField] protected int normalAppeal = 0, normalJam = 0;
    //Super
    [SerializeField] protected Contest superCondition = 0;
    [SerializeField] protected int superAppeal = 0, superJam = 0;
    //Spectacular
    [SerializeField] protected Contest spectacularCondition = 0;
    [SerializeField] protected int spectacularAppeal = 0, spectacularJam = 0;

    [Header("Status:")]
    [SerializeField] protected bool hasStatus = false;
    [SerializeField] protected Condition statusCondition = null;
    [SerializeField] protected float applyChange = 0;
    [SerializeField] protected bool statusHit = false;
    [SerializeField] Chat statusHitChat = null, statusFailedChat = null;

    [Header("Action Operation:")]
    protected float[] damagePerTarget = new float[0], damageApplied = new float[0], damageOverTime = new float[0];

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
    public void SetTargetIndex(int set)
    {
        targetIndex = set;
    }

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
        normalCondition = (Contest)set[0];
        normalAppeal = set[1];
        normalJam = set[2];
    }

    public void SetSuperContests(int[] set)
    {
        superCondition = (Contest)set[0];
        superAppeal = set[1];
        superJam = set[2];
    }

    public void SetSpectacularContests(int[] set)
    {
        spectacularCondition = (Contest)set[0];
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
        return new int[] { (int)normalCondition, normalAppeal, normalJam };
    }

    public int[] GetSuperContests()
    {
        return new int[] { (int)superCondition, superAppeal, superJam };
    }

    public int[] GetSpectacularContests()
    {
        return new int[] { (int)spectacularCondition, spectacularAppeal, spectacularJam };
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
            foreach (Spot s in SetupTargets(targetIndex))
            {
                if (s != null)
                {
                    if (s.GetActivePokemon() != null)
                        targetPokemon.Add(s.GetActivePokemon());
                }
            }

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
            if (chatOnActivation[i] != null)
            {
                result[i] = chatOnActivation[i].GetChat();

                result[i].AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                result[i].AddToOverride("<POKEMON_MOVE>", moveName);
            }
        }

        return result;
    }

    protected override Spot[] SetupTargets(int Index)
    {
        targetPokemon.Clear();
        List<Spot> result = new List<Spot>();
        Spot[] toCheck = BattleMaster.instance.GetSpots();

        if (hitType == HitType.One)
        {
            //The Spot relativ to the index is returned
            foreach (Spot s in toCheck)
            {
                if (s == null)
                    continue;

                if (s.GetSpotNumber() == Index)
                {
                    result.Add(s);
                    break;
                }
            }
        }
        else if (hitType == HitType.AllOneSide)
        {
            bool checkDone = false;

            foreach (Spot s in toCheck)
            {
                if (s.GetSpotNumber() == Index)
                    result.Add(s);
            }

            while (!checkDone)
            {
                checkDone = true;

                foreach (Spot s in result)
                {
                    bool allowSelfTarget = true;

                    if (s.GetLeft() != null)
                    {
                        if (s.GetLeft().GetActivePokemon() == currentPokemon && !selfTargetable)
                            allowSelfTarget = false;

                        if (!result.Contains(s.GetLeft()) && allowSelfTarget)
                        {
                            result.Add(s.GetLeft());
                            checkDone = false;
                        }
                    }

                    allowSelfTarget = true;

                    if (s.GetRight() != null)
                    {
                        if (s.GetRight().GetActivePokemon() == currentPokemon && !selfTargetable)
                            allowSelfTarget = false;

                        if (!result.Contains(s.GetRight()))
                        {
                            result.Add(s.GetRight());
                            checkDone = false;
                        }
                    }
                }
            }
        }
        else if (hitType == HitType.AllAdjacentOneSide)
        {
            foreach (Spot s in toCheck)
            {
                if (s.GetSpotNumber() == Index)
                {
                    if (s.GetActivePokemon() != currentPokemon)
                        result.Add(s);

                    result.Add(s.GetLeft());
                    result.Add(s.GetRight());

                    break;
                }
            }
        }
        else if (hitType == HitType.AllAdjacent)
        {
            foreach (Spot s in toCheck)
            {
                if (s.GetActivePokemon() == currentPokemon)
                {
                    if (s.GetLeft() != null)
                    {
                        result.Add(s.GetLeft());
                        result.Add(s.GetLeft().GetFront());
                    }
                    if (s.GetRight() != null)
                    {
                        result.Add(s.GetRight());
                        result.Add(s.GetRight().GetFront());
                    }
                    if (s.GetFront() != null)
                    {
                        result.Add(s.GetFront());

                        if (s.GetLeft() == null)
                            result.Add(s.GetFront().GetRight());
                        if (s.GetRight() == null)
                            result.Add(s.GetFront().GetLeft());
                    }

                    break;
                }
            }
        }
        else if (hitType == HitType.All)
        {
            foreach (Spot s in toCheck)
            {
                if (s.GetActivePokemon() != currentPokemon)
                    result.Add(s);
                else if (s.GetActivePokemon() == currentPokemon && selfTargetable)
                    result.Add(s);
            }
        }
        return result.ToArray();
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
                        if (Mathf.Clamp(damageApplied[i] + damageOverTime[i], 0, damagePerTarget[i]) == damagePerTarget[i])
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