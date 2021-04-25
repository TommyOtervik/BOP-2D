using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Rogue : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10.0f;
    private float distanceTravelled;
    private Vector2 lastPosition;
    private const float MAXTravelDistance = 20.0f;
    
    
    // Endre dette later, virker snodig å ha player referanse i alt som kan skade player. 
    private const string PLAYER_NAME = "Player";
    
    private int damageAmount = 10;
    
    
    // Use this for initialization

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
        // Treffer platform layer
        if (collision.gameObject.layer == 10)
        {   
            Destroy(gameObject);
        }
        // Treffer enemy layer
        else if (collision.gameObject.layer == 12)
        {
            DamageBroker.EnemyTakesDamage(damageAmount, collision.gameObject);
            Destroy(gameObject);
        }
    }
    
    
}