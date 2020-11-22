#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public enum EvolutionMethod { None, Level, Item, Trade, LearnedMove }

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon", order = 0)]
public class Pokemon : ScriptableObject
{
    #region Values:
    [Header("Object Reference:")]
    [SerializeField] private bool isInstantiated = false;
    [SerializeField] private string pokemonName = "";
    [SerializeField] private Type[] types = new Type[2];
    [SerializeField] private Ability ability = null;
    [SerializeField] private HoldableItem itemInHand = null;
    [SerializeField, TextArea] private string description = "";

    [Header("Conditions:")]
    [SerializeField] private ConditionOversight oversight = null;

    [Header("Stats:")]
    [SerializeField] private int health = 0;
    [SerializeField] private float currentHealth = 0;
    [SerializeField] private int defence = 0, specialDefence = 0;
    [SerializeField] private int attack = 0, specialAttack = 0;
    [SerializeField] private int speed = 0;
    [SerializeField] private int level = 0, maxExp = 0;
    [SerializeField] private int currentExp = 0;

    [Header("Evolotion:")]
    [SerializeField] private EvolutionMethod method = 0;
    [SerializeField] Pokemon evolveTo = null;
    [SerializeField] int evolutionLevel = 0;

    [Header("Moves:")]
    [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];
    [Header(" -- Learnable:")]
    [SerializeField] private int[] key = new int[0];
    [SerializeField] private PokemonMove[] result = new PokemonMove[0];
    private Dictionary<int, PokemonMove> learnableMoves = new Dictionary<int, PokemonMove>();

    [Header("Visual:")]
    [SerializeField] private GameObject prefab = null;
    private GameObject spawnedObject = null;
    [Header(" -- Animation:")]
    private Animator anim = null;

    private void OnValidate()
    {
        #region Typing Check
        if (types.Length > 2)
        {
            types = new Type[2];
            Debug.Log("A Pokemon can max have two types!");
        }
        else if (types.Length < 1)
        {
            types = new Type[1];
            Debug.Log("A Pokemon must have at least one type!");
        }
        else if (types.Length == 1)
        {
            Type holder = types[0];
            types = new Type[2];
            types[0] = holder;
        }
        if (types.Length == 2)
        {
            if (types[0] == types[1] && types[0] != null)
                types[1] = null;
            else if (types[0] == null && types[1] != null)
            {
                types[0] = types[1];
                types[1] = null;
            }
        }
        #endregion
        #region Move Check
        if (result.Length > key.Length)
        {
            int[] copy = key;
            key = new int[result.Length];
            for (int i = 0; i < copy.Length; i++)
                key[i] = copy[i];
        }
        else if (result.Length < key.Length)
        {
            int[] copy = key;
            key = new int[result.Length];
            for (int i = 0; i < key.Length; i++)
                key[i] = copy[i];
        }
        #endregion
        #region Health Check
        currentHealth = health;
        #endregion
    }
    #endregion
    #region Getters
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

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetSpecialAttack()
    {
        return specialAttack;
    }

    public int GetDefence()
    {
        return defence;
    }

    public int GetSpecialDefence()
    {
        return specialDefence;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public Type[] GetTypes()
    {
        return types;
    }

    public PokemonMove[] GetMoves()
    {
        return learnedMoves;
    }

    public PokemonMove GetMoveByIndex(int index)
    {
        if (index >= 0 && index <= 4)
            return learnedMoves[index];
        else
            return null;
    }

    public ConditionOversight GetConditionOversight()
    {
        if (!oversight.GetIsInstantiated())
            oversight = oversight.GetConditionOversight();

        return oversight;
    }
    #endregion
    #region Setters
    public void SetAbility(Ability toSet)
    {
        ability = toSet;
    }

    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }
    #endregion
    #region Out
    public void SpawnPokemon(Transform transform, bool back)
    {
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(prefab);
            spawnedObject.transform.position = transform.position;
            spawnedObject.transform.rotation = transform.rotation;
            spawnedObject.transform.parent = transform;
        }
    }

    public void DespawnPokemon()
    {
        Destroy(spawnedObject);
        spawnedObject = null;
    }

    public bool IsSameType(TypeName typeName)
    {
        if (types[0].GetTypeName() == typeName)
            return true;
        else if (types[1] != null)
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
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);
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