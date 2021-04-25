using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyPassive : Enemy, IDamageable
{
    [SerializeField] public float speed;
    [SerializeField] public float lineOfSight;
    private Transform player;
    private const int OffsetY = 2;
    private const string PLAYER_NAME = "Player";
    private int collisionDamageAmount = 10;
    private int maxHealth = 30;
    private int currentHealth;
    private Collider2D collider;
    private Animator enemyAnim;

    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        collider = GetComponent<BoxCollider2D>();
        enemyAnim = GetComponent<Animator>();
        enemyAnim.SetBool("isDead", false);
        currentHealth = maxHealth;
        
    }
    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSight)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(player.position.x, player.position.y + OffsetY), speed*Time.deltaTime);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(collisionDamageAmount);
        }
        Physics2D.IgnoreCollision(collider, collision.collider, true);
    }
    
    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        enemyAnim.SetTrigger("wasHit");
        
        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        if (gameObject != null)
        {
            base.MakeLoot();
            enemyAnim.SetBool("isDead", true);
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        DamageBroker.AddToEnemyList(this);
    }

    private void OnDisable()
    {
        DamageBroker.RemoveEnemyFromList(this);
    }
    
    public GameObject GetEnemyGameObject()
    {
        return collider.gameObject;
    }
}
