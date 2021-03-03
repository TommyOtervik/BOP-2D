using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour
{

    [SerializeField]
    private Transform spawnPoint;

    private const string PLAYER_TAG = "Player";
    private const string KILL_FLOOR_HIT_KEY = "KillFloorHit";

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag(PLAYER_TAG))
        {
            EventManager.TriggerEvent(KILL_FLOOR_HIT_KEY);
        }
    }

}
