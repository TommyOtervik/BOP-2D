using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tilhører Player.
 *  Brukes til å lagre data i binær format. Serializable.
 *   
 *   @AOP - 225280
 */
[System.Serializable]
public class PlayerData
{
    private int currentHealth;


    public PlayerData(Player player)
    {
        currentHealth = player.GetCurrentHealth();

    }


    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }
}
