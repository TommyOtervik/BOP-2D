using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Dette skriptet tilhører Cultist.
 * Håndterer skade.
 * 
 * @AOP - 225280
 */
public class CultistHitBox : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    [SerializeField] private int damageAmount = 30; // Skade
    private UnityAction cultistDeadListener;        // Lytter om Cultst er død, brukes for å deaktivere HitBox


    private void Awake()
    {
        // Deaktiver HitBox om Cultist dør
        cultistDeadListener = new UnityAction(DisableThis);
    }

    // Gjør skade mot spilleren, om han treffer
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
            DamageBroker.CallTakeDamageEvent(damageAmount);
    }

    private void DisableThis()
    {
         GetComponent<BoxCollider2D>().gameObject.SetActive(false);   
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.CULTIST_DEAD, cultistDeadListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.CULTIST_DEAD, cultistDeadListener);
    }
}
