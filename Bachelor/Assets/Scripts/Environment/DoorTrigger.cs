using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
