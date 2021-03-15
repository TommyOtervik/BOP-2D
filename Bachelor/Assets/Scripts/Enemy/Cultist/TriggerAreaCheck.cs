using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    public static event Action<Transform, bool, bool> UpdateAreaEnter;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            gameObject.SetActive(false);
            UpdateAreaEnter?.Invoke(collider.transform, true, true);
        }
    }
}
