using System.Collections;
using UnityEngine;

public class IcePistol : AbilitiesScript
{
    public void UseIcePistol(Unit targetUnit)
    {
        targetUnit.TakeDamage(damage);

        targetUnit.isFrozen = true;
        targetUnit.lingeringDamageTurns = lingeringDamageTurnsLeft;

        // Add Other Effects Here
    }
}
