using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skriptet tilhører dør objektet som orginalt er låst.
 * Sjekker om spilleren er innenfor sonen.
 *   
 * @AOP - 225280
 */
public class DoorTrigger : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private bool isInside;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_NAME))
        {
            isInside = true;
            GameManager.RegisterInsideTrigger(isInside);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_NAME))
        {
            isInside = false;
            GameManager.RegisterInsideTrigger(isInside);
        }
    }
}
