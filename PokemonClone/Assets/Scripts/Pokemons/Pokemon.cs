using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class Pokemon : ScriptableObject
{
    #region Values:
    [Header("Object Reference:")]
    [SerializeField] private string pokemonName = "";
    [SerializeField] protected Type[] types = new Type[2];

    [Header("Stats:")]
    [SerializeField] protected int health = 0;
    protected int currentHealth = 0;
    [SerializeField] protected int defence = 0, specialDefence = 0;
    [SerializeField] protected int attack = 0, specialAttack = 0;
    [SerializeField] protected int speed = 0;
    [SerializeField] protected int level = 0, exp = 0;

    [Header("Moves:")]
    [SerializeField] private PokemonMove[] learnedMoves = new PokemonMove[4];
    [Header(" -- Learnable:")]
    [SerializeField] protected int[] key = new int[0];
    [SerializeField] protected PokemonMove[] result = new PokemonMove[0];
    protected Dictionary<int, PokemonMove> learnableMoves = new Dictionary<int, PokemonMove>();

    [Header("Display:")]
    public Vector2 spriteOffset = Vector2.zero;

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

        currentHealth = health;
    }
    #endregion

    #region Getters/Setters
    public string GetName()
    {
        return pokemonName;
    }

    public int GetCurrentHealth()
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
    #endregion

    public void Damage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);
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
}
