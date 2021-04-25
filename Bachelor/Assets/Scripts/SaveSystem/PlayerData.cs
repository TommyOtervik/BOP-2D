using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    private int currentHealth;
    private float[] position;

    public PlayerData(Player player)
    {
        currentHealth = player.GetCurrentHealth();

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }


    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
