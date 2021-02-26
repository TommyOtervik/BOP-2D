using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveObjectMove : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10.0f;
    // Use this for initialization

    public void Init(Vector2 direction)
    {
        this.direction = direction;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D (Collider2D target) {
        Destroy(gameObject);
    }
    

}



