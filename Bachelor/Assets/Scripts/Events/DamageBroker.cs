using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageBroker
{
    public static event Action<int> TakeDamageEvent;

    private static List<IEnemy> enemies = new List<IEnemy>();

    public static void CallTakeDamageEvent(int damage)
    {
        TakeDamageEvent?.Invoke(damage);
    }



    public static void EnemyTakesDamage(int damage, GameObject enemyHit)
    {

    }


    public static void AddToEnemyList(IEnemy enemy)
    {
        enemies.Add(enemy);
    }
}


