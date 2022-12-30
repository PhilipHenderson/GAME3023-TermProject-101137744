using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : AbilitiesScript
{
    public void UseBasicAttack(Unit targetUnit)
    {
        targetUnit.TakeDamage(damage);
    }
}
