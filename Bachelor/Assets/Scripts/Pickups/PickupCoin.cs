using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PickupCoin : Pickup
{
    private int coinValue = 1;
    
    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            PickupBroker.CallCoinPickupEvent(coinValue);
            Destroy(gameObject);
        }
    }
    
}
