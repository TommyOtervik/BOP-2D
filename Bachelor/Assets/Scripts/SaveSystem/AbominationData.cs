using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Tilhører Abomination.
 *  Brukes til å lagre data i binær format. Serializable.
 *   
 *   @AOP - 225280
 */

[System.Serializable]
public class AbominationData
{
    private int currentHealth;


    public AbominationData(AbominationMiniBoss abom)
    {
        this.currentHealth = abom.GetCurrentHealth();
    }

    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }
}
