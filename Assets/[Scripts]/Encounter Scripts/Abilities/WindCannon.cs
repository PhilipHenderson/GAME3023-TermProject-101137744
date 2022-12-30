using System.Collections;
using UnityEngine;

public class WindCannon : AbilitiesScript
{
    public void UseWindCannon(Unit targetUnit)
    {
        targetUnit.isBlownAway = true;

        // Add Other Effects Here
    }
}
