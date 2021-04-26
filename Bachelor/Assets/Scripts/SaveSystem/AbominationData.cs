using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
