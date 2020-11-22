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
    None,
    Tough,
    Cute,
    Clever,
    Beutiful,
    Cool
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
    [SerializeField] protected Contest contest = 0;
    [SerializeField] protected int maxPP = 0;
    protected int currentPP = 0;
    [SerializeField] protected int power = 0, accuracy = 0;

    [Header("Interaction:")]
    [SerializeField] protected bool makesContact = false;
    [SerializeField] protected bool affectByProtect = false, affectByMagicCoat = false, affectBySnatch = false, affectByMirrorMove = false, affectByKingsRock = false;

    [Header("Status:")]
    [SerializeField] protected Condition canApplyCondition = null;
    [SerializeField] protected float applyChange = 0;
    [SerializeField] protected bool statusHit = false;
    [SerializeField] Chat statusHitChat = null, statusFailedChat = null;

    [Header("Action Operation:")]
    protected bool active = false, done = false;
    protected float[] damagePerTarget = new float[0], damageApplied = new float[0], damageOverTime = new float[0];

    [Header("Instatiation:")]
    protected bool isInstantiated = false;

    private void OnValidate()
    {
        if (maxPP < 0)
        {
            maxPP = 0;
            Debug.LogError("Max PP of " + moveName + " should be more then 0!");
        }

        currentPP = maxPP;
    }
    #endregion
    #region Setters
    public void SetCurrentPokemon(Pokemon pokemon)
    {
        currentPokemon = pokemon;
    }

    public void SetTargetPokemon(Pokemon[] targets)
    {
        targetPokemon = targets;
    }

    public void SetIsInstantiated()
    {
        isInstantiated = true;
    }
    #endregion
    #region Getters
    public string GetName()
    {
        return moveName;
    }

    public bool GetActive()
    {
        return active;
    }

    public bool IsDone()
    {
        return done;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public PokemonMove GetPokemonMove()
    {
        PokemonMove result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(this);
            result.SetIsInstantiated();
        }

        return result;
    }

    public new Type GetType()
    {
        return type;
    }

    public Category GetCategory()
    {
        return category;
    }
    #endregion
    #region Out
    protected override void TransferInformationToChat()
    {
        for (int i = 0; i < chatOnActivation.Length; i++)
        {
            Chat c = chatOnActivation[i];
            if (c != null)
            {
                if (!c.GetIsInstantiated())
                {
                    chatOnActivation[i] = Instantiate(c);
                    chatOnActivation[i].SetIsInstantiated();
                    c = chatOnActivation[i];
                }

                if (currentPokemon != null)
                    c.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                c.AddToOverride("<POKEMON_MOVE>", moveName);
            }
        }
    }

    public new IEnumerator Activate()
    {
        if (!active)
        {
            TransferInformationToChat();
            SendChatsToMaster();

            active = true;
            done = false;
        }

        if (category != Category.Status)
        {
            if (targetPokemon.Length > 0 && currentPokemon != null && power > 0)
            {
                if (category == Category.Physical || category == Category.Special)
                {
                    float attack = currentPokemon.GetSpecialAttack();

                    if (category == Category.Physical)
                        attack = currentPokemon.GetAttack();

                    damagePerTarget = new float[targetPokemon.Length];

                    for (int i = 0; i < targetPokemon.Length; i++)
                    {
                        Pokemon p = targetPokemon[i];
                        float defence = p.GetSpecialDefence();

                        if (category == Category.Physical)
                            defence = p.GetDefence();

                        int damage = BattleMathf.CalculateDamage(
                            currentPokemon.GetLevel(),
                            attack,
                            defence,
                            power,
                            BattleMathf.CalculateModifiers(
                                currentPokemon, p, this, (targetPokemon.Length == 1)));

                        damagePerTarget[i] = damage;
                    }

                }
            }
        }
        else
        {
            if (canApplyCondition != null)
            {
                if (Random.Range(0, 100) <= applyChange)
                {
                    foreach (Pokemon target in targetPokemon)
                    {
                        Condition toApply = canApplyCondition.GetCondition();
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
    #region IEnumerator
    private IEnumerator Operation()
    {
        if (category != Category.Status)
        {
            if (damagePerTarget.Length > 0)
            {
                float divide = 200 * BattleMaster.instance.GetSecPerPokeMove();

                float[] damageApplied = new float[targetPokemon.Length];
                float[] damageOverTime = new float[targetPokemon.Length];

                for (int i = 0; i < targetPokemon.Length; i++)
                    damageOverTime[i] = damagePerTarget[i] / divide;

                while (damageApplied[0] < damagePerTarget[0])
                {
                    for (int i = 0; i < targetPokemon.Length; i++)
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
