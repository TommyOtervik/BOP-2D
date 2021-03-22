using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private EnemyCultist enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyCultist>();
    }

    // Hvis spilleren går inn i området skal AI søke etter spilleren,
    // setter data i EnemyCultist (Om hen går innenfor).
    // Setter target til spillerens pos, inRange og hotZone (er spilleren innenfor 
    // denne sonen, skal AI følge etter)
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            gameObject.SetActive(false);
            enemyParent.SetTarget(collider.transform);
            enemyParent.SetInRange(true);
            enemyParent.SetHotZone(true);
        }
    }
}
