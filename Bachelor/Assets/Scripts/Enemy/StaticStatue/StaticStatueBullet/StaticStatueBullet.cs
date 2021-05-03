﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticStatueBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private float distanceTravelled;
    private Vector2 lastPosition;
    private const float MAXTravelDistance = 40.0f;
    
    
    
    private const string PLAYER_NAME = "Player";
    
    private int damageAmount = 10;
    
    
    void Start()
    {
        distanceTravelled = 0;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // 
        if (distanceTravelled > MAXTravelDistance )
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.name == PLAYER_NAME)
        {
            // Hvis sant, gjør skade til spilleren
            DamageBroker.CallTakeDamageEvent(damageAmount);
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
