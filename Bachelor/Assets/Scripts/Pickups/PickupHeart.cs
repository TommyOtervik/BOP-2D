using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PickupHeart : MonoBehaviour
{
    private int healingValue = 20;
    private const string PLAYER_NAME = "Player";
    private bool up = true;
    private Vector2 startPosition;
    private Vector2 upperBound;
    private Vector2 lowerBound;
    private float speed = 1.0f;
    private bool movingUp = true;
    private float upperBoundYOffset = 0.6f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        upperBound = new Vector2(startPosition.x, startPosition.y + upperBoundYOffset);
        lowerBound = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            PickupBroker.CallHealthPickupEvent(healingValue);
            Destroy(gameObject);
        }
    }

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
}
