using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skirptet håndterer KillFloor.
 *  Sjekker om spilleren treffer "gulvet", og sender et Event om hen gjør det.
 *   
 * @AOP - 225280
 */
public class KillFloor : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag(PLAYER_TAG))
            EventManager.TriggerEvent(EnumEvents.KILL_FLOOR_HIT);
    }

}
