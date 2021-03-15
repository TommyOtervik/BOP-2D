using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageBroker
{
    public static event Action<int> TakeDamageEvent;

   
    private static List<IDamageable> enemies = new List<IDamageable>();

    public static void CallTakeDamageEvent(int damage)
    {
        TakeDamageEvent?.Invoke(damage);
    }



    public static void EnemyTakesDamage(int damage, GameObject enemyGameObject)
    {
       
        foreach (IDamageable e in enemies)
        {
            if (GameObject.ReferenceEquals(e.GetEnemyGameObject(), enemyGameObject))
            {
                e.TakeDamage(damage);
                break;
            }
                
        }
    }


    public static void AddToEnemyList(IDamageable enemy)
    {
        enemies.Add(enemy);
    }

    public static void RemoveEnemyFromList(IDamageable enemy)
    {
        enemies.Remove(enemy);
    }
}


