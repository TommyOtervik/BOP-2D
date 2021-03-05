using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CultistHitBox : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private readonly int damageAmount = 20;

    public static event Action<int> CultistDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            CultistDamage?.Invoke(damageAmount);
        }
    }


}
