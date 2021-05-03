using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBatBulletMovementAndDamage : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10.0f;
    private float distanceTravelled;
    private Vector2 lastPosition;
    private float MAXTravelDistance = 30.0f;
    
    
    private const string PLAYER_NAME = "Player";
    
    private int damageAmount = 1;
    
    

    public void Init(Vector2 direction)
    {
        this.direction = direction;
    }

    void Start()
    {
        distanceTravelled = 0;
        lastPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // 
        if (distanceTravelled > MAXTravelDistance )
        {
            Destroy(gameObject);
        }

        
    }
    
    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            // Hvis sant, gjør skade til spilleren
            DamageBroker.CallTakeDamageEvent(damageAmount);
            Destroy(gameObject);
        }
        
    }
    
    
}







