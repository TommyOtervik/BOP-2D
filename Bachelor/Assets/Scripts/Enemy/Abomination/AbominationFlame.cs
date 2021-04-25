using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationFlame : MonoBehaviour
{

    private const int DAMAGE_AMOUNT = 1;
    private const string PLAYER_NAME = "Player";

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(DAMAGE_AMOUNT);
        }
    }
}
