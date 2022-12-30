using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AbilitiesScript
{
    public IEnumerator UseFlee()
    {
        // Add Flea from the opponent Code Here

        yield return new WaitForSeconds(2.0f);
    }
}
