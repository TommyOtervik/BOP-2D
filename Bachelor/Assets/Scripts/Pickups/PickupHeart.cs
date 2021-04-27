using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PickupHeart : Pickup
{
    private int healingValue = 10;
    

    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            PickupBroker.CallHealthPickupEvent(healingValue);
            Destroy(gameObject);
        }
    }
    /*
    void Move()
    {
        if (movingUp)
        {
            if (transform.position.y >= upperBound.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, lowerBound, speed * Time.deltaTime);
                movingUp = false;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, upperBound, speed * Time.deltaTime);
            }

            
        }
        
        else if (!movingUp)
        {
            if (transform.position.y <= lowerBound.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, upperBound, speed * Time.deltaTime);
                movingUp = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, lowerBound, speed * Time.deltaTime);
            }

            
        }

    }
    */
}
