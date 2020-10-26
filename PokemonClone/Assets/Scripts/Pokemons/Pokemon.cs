using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pokemon : ScriptableObject
{
    #region Values:
    [Header("Object Reference:")]
    public string name; 
    public int currentHealth;

    [Header("Stats:")]
    [SerializeField] protected int health;
    [SerializeField] protected int defence, specialDefence;
    [SerializeField] protected int attack, specialAttack;

    [Header("Display:")]
    public Vector2 spriteOffset = Vector2.zero;
    #endregion

    #region Getters/Setters
    public int GetHealth()
    {
        return health;
    }
    #endregion
}
