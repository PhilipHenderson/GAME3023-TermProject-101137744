using System.Collections;
using UnityEngine;

public class FireBlast : AbilitiesScript
{
    public void UseFireBlast(Unit targetUnit)
    {
        targetUnit.TakeDamage(damage);

        targetUnit.isOnFire = true;
        targetUnit.lingeringDamageTurns = lingeringDamageTurnsLeft;

        // Add Other Effects Here
    }
}