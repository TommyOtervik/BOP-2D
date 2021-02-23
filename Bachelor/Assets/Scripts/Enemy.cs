﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    #region Public varaibles

    [Header("Enemy Info")]
    public int maxHealth = 100;

    // Sid
    public Transform rayCast;
    public LayerMask raycastMask;
    public float attackDistance; // Min. distance for attack
    public float rayCastLength;
    public float moveSpeed;
    public float timer; // Timer for cooldown between attacks

    public Transform leftLimit;
    public Transform rightLimit;
    #endregion


    #region Private Variables 
    Animator anim;


    [SerializeField]
    int currentHealth;
    // Sid
    private RaycastHit2D hit;
    private Transform target;
    private float distance; // Store distance b/w enemy and player
    private bool attackMode;
    private bool inRange; // Check if player is in range
    private bool cooling; // Check if enemy is cooling after attack
    private float intTimer;
    #endregion


    void Awake()
    {
        SelectTarget(); 

        intTimer = timer;

        anim = GetComponent<Animator>();
     
        currentHealth = maxHealth;


    }

    void Update()
    {

        if (!attackMode)
            Move();

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            SelectTarget();
        }

        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        // When player is dectected
        if (hit.collider != null)
            EnemyLogic();
        else if (hit.collider == null)
            inRange = false;

        if (inRange == false)
            StopAttack();
       

    }


    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
            StopAttack();
        else if (attackDistance >= distance && cooling == false)
            Attack();


        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }

    }

    private void Cooldown()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void Attack()
    {
        timer = intTimer; // Reset timer when player enter attack range
        attackMode = true; // To check if enemey can still attack or not

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    private void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPos = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    private void RaycastDebugger()
    {
        if (distance > attackDistance)
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        else if (attackDistance > distance)
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt anim


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Die anim


        // Disable the enemy
        GetComponent<Collider2D>().enabled = false;
       

        this.enabled = false;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.transform;
            inRange = true;
            Flip();
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
    }


    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


    private void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);


        if (distanceToLeft > distanceToRight)
            target = leftLimit;
        else
            target = rightLimit;

        Flip();

    }

    private void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > target.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }
}
