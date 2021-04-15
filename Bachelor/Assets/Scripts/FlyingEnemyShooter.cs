﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyShooter : MonoBehaviour, IDamageable
{
    [SerializeField] public float speed;
    [SerializeField] public float lineOfSight;
    [SerializeField] public float shootingRange;
    public float fireRate = 1.0f;
    private float nextFireTime;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject attackPoint;
    private Transform player;
    private int offsetY = 2;
    private const string PLAYER_NAME = "Player";
    private int collisionDamageAmount = 10;
    private int maxHealth = 30;
    private int currentHealth;
    private Collider2D collider;

    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        collider = GetComponent<CircleCollider2D>();
        currentHealth = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSight && distanceFromPlayer > shootingRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(player.position.x, player.position.y + offsetY), speed*Time.deltaTime);

        }
        else if (distanceFromPlayer <= shootingRange && nextFireTime < Time.time)
        {
            Instantiate(bulletPrefab, attackPoint.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
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
        
        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        if (gameObject != null)
        {
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

