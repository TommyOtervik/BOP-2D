using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationMiniBoss : MonoBehaviour
{

    private Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collider, collision.collider);
        }
    }
}
