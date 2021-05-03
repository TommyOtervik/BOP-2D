using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skriptet tilhører CrossbowCultist.
 * 
 */
public class Bolt : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";


    private Vector2 direction;                      // Retning man skyter i
    private readonly float speed = 25.0f;           // Hastigheten
    private float distanceTravelled;                // Distansen den har reist
    private const float MAXTravelDistance = 40.0f;  // Max. distanse 
    private Vector2 lastPosition;                   
    private readonly int damageAmount = 35;         // Skade den skal gjøre mot spilleren   
     

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
        // Sender pilen bortover
        transform.Translate(direction * speed * Time.deltaTime);
        
        // Teller hvor langt den har reist
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Når Max. distanse -> Slett objektet
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
        }
        // All annen type kollisjon, ødelegg objektet
        Destroy(gameObject);
    }
    
    
}