using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skriptet håndterer skaden som blir gjort
 * mot spilleren, sender videre til DamageBroker.
 * 
 * Denne blir aktivert i animasjonen.
 * 
 * @ AOP - 225280
 */
public class AbominationHitBox : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    [SerializeField]
    private int damageAmount = 10;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
            DamageBroker.CallTakeDamageEvent(damageAmount);
    }
}
