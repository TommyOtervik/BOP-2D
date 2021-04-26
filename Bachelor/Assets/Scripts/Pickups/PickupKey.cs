using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PickupKey : Pickup
{

    //private void Awake()
    //{
    //    GameManager.RegisterKey(this);
    //}

    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            GameManager.PlayerGrabbedKey(this);
            PickupBroker.CallKeyPickupEvent();
            Destroy(gameObject);
        }
    }
    
}

