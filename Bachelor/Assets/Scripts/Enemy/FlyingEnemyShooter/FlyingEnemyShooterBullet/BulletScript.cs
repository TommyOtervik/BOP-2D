using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private GameObject target;
    public float speed;
    private Rigidbody2D bulletRb;
    private int offsetY = 5;
    private const string PLAYER_NAME = "Player";
    private int collisionDamageAmount = 10;

    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player");
        Vector2 moveDirection = (target.transform.position - transform.position).normalized * speed;
        bulletRb.velocity = new Vector2(moveDirection.x, moveDirection.y + offsetY);
        Destroy(this.gameObject, 2);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(collisionDamageAmount);
            Destroy(gameObject);
        }
    }
}
