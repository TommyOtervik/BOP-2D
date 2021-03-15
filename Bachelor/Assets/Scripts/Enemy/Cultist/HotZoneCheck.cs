using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private bool inRange;
    private const string PLAYER_NAME = "Player";


    private void Update()
    {
        if(inRange)
        { 
            EventManager.TriggerEvent(EnumEvents.FLIP_CULTIST);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            inRange = false;
            gameObject.SetActive(false);

            EventManager.TriggerEvent(EnumEvents.HOT_ZONE_EXIT);
        }
    }

}
