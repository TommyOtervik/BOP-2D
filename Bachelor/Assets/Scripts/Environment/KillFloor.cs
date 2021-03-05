using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour
{

    [SerializeField]
    private Transform spawnPoint;

    private const string PLAYER_TAG = "Player";


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag(PLAYER_TAG))
        {
            EventManager.TriggerEvent(EnumEvents.KILL_FLOOR_HIT);
        }
    }

}
