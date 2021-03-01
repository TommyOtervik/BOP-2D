using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveObjectMove : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10.0f;
    private float distanceTravelled;
    private Vector2 lastPosition;
    private const float MAXTravelDistance = 40.0f;
    
    
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
    
    void OnTriggerEnter2D (Collider2D target) {
        Destroy(gameObject);
    }
    

}



