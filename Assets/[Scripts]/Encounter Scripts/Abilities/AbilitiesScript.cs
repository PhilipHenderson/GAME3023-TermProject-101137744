using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/New Ability")]
public class AbilitiesScript : ScriptableObject
{
    [Header("Ability Info")]
    public string AbilityName;
    [TextArea(2,5)]
    public string AbilityDescription;
    public int levelRequirement;
    public int damage;
    public int lingeringDamage;
    public int lingeringDamageTurnsLeft;
    public int abilityCost;

    // Add Base Ability info here
}

