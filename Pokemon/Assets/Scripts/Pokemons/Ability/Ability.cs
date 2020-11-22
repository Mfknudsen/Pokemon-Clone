using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityName {Test }

[CreateAssetMenu(fileName = "Ability", menuName = "Pokemon/Create New Ablility", order = 2)]
public class Ability : ScriptableObject
{
    [SerializeField] private AbilityName abilityName = 0;
    [SerializeField, TextArea] private string description = "";
}
