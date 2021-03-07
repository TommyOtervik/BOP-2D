﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    public Rigidbody2D body2d;
    public float leftPushRange;
    public float rightPushRange;
    public float velocityThreshold;
    public float pushStrength = 10.0f;
    
    // Endre dette later, virker snodig å ha player referanse i alt som kan skade player. 
    private const string PLAYER_NAME = "Player";
    
    private int damageAmount = 20;

    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        body2d = GetComponent<Rigidbody2D>();
        body2d.angularVelocity = velocityThreshold;
    }

    void Update()
    {
        Push();
    }

    void Push()
    {
        if (transform.rotation.z > 0 && transform.rotation.z < rightPushRange && (body2d.angularVelocity > 0) &&
            body2d.angularVelocity < velocityThreshold)
        {
            body2d.angularVelocity = velocityThreshold;
        } 
        else if (transform.rotation.z < 0 && transform.rotation.z > leftPushRange && (body2d.angularVelocity < 0) &&
                 body2d.angularVelocity > velocityThreshold * -1)
        {
            body2d.angularVelocity = velocityThreshold * -1;
        }
    }
    // PUSH STRENGTH ER UNTESTED, FIKS I MÅRRA 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == PLAYER_NAME)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRigidbody.AddForce(awayFromPlayer * pushStrength, ForceMode.Impulse);
            
        } 
        
        
        
        
             
           
            
           


        
        
    }
}
