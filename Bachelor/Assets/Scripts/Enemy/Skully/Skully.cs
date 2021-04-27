﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Skully : Enemy, IDamageable
{
    /*
     *HUSK PÅ AT DU HAR COMMENTA UT STUFF I AWAKE METODEN
        VELG ATTACK PATTERN I DET HAN LANDER / ER I SPAWN POINT?
        TRENGER IKKE VELGE ATTACK2 fordi den er kobla til ATTACK1 ?
     *
     *
     * 
     */
    
    // Collision enter etterpå
    private int maxHealth = 30;
    private int currentHealth;
    private Collider2D collider;
    private float speed = 10.0f;

    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointDownLeft;
    [SerializeField] private Transform attackPointDownRight;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform upperLeft;
    [SerializeField] private Transform upperRight;
    [SerializeField] private Transform upperCenter;

    [SerializeField] private GameObject bulletPrefab;
    private float attackRate = 0.4f;
    private Transform target;
    private bool attackInProgress = false;
    private bool leftToRightSprayFinished = false;
    private bool rightToLeftSprayFinished = false;
    private bool rangedAttacksFinished = false;
    
    private UnityAction hitUpperLeftListener;
    private UnityAction hitUpperRightListener;
    private UnityAction hitUpperCenterListener;
    private UnityAction hitSpawnPointListener;

// Start is called before the first frame update
    void Awake()
    {
        hitUpperLeftListener = new UnityAction(Attack2);
        hitUpperRightListener = new UnityAction(Attack1);
        //hitUpperCenterListener = new UnityAction(DiveAttack());
        //hitSpawnPointListener = new UnityAction(ChooseAttack());
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        transform.position = spawnPoint.position;
        target = upperRight;
    }

    // Update is called once per frame
    void Update()
    {
        MovementCheck();
    }
    
    
    // Hit Right
    void Attack1()
    {
        int bulletAmount = 9;
        /*
        if (!attackInProgress && !rightToLeftSprayFinished)
        {
            StartCoroutine(SpawnBulletsDown(bulletAmount, attackRate, "Left"));
        }
        */
        StartCoroutine(SpawnBulletsDown(bulletAmount, attackRate, "Left"));
        
    }

    // Hit left
    void Attack2()
    {
        int bulletAmount = 9;
        
        /*
        if (!attackInProgress && !leftToRightSprayFinished && rightToLeftSprayFinished)
        {
            StartCoroutine(SpawnBulletsDown(bulletAmount, attackRate, "Right"));
        }
        */
        StartCoroutine(SpawnBulletsDown(bulletAmount, attackRate, "Right"));
    }

    IEnumerator SpawnBulletsDown(int amount, float delay, string sprayDirection)
    {
        attackInProgress = true;
        if (sprayDirection == "Left")
        {
            target = upperLeft;
        }
        else
        {
            target = upperRight;
        }
        
        
        GameObject tempBullet;
        for (int i = 0; i < amount; i++)
        {
            tempBullet = Instantiate(bulletPrefab, attackPointDown.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().Init(Vector2.down);

            yield return new WaitForSeconds(delay);

        }

        attackInProgress = false;
        if (sprayDirection == "Left")
        {
            rightToLeftSprayFinished = true;
        }
        else
        {
            leftToRightSprayFinished = true;
            // Går bra fordi komboen starter alltid fra høyre og avsluttes på høyre
            rangedAttacksFinished = true;
        }


    }


    void MovePointToPoint(Transform current, Transform target)
    {
        if (current.position != target.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            target = null;
        }
    }

    void MovementCheck()
    {
        if (transform.position == upperLeft.position)
        {
            // Sjekke stuff 
            if (!attackInProgress && !leftToRightSprayFinished && rightToLeftSprayFinished)
            {
                EventManager.TriggerEvent(EnumEvents.SKULLY_HIT_UPPER_LEFT);
            }
        }
        else if (transform.position == upperRight.position)
        {
            Debug.Log("Big true");
            if (!attackInProgress && !rightToLeftSprayFinished)
            {
                EventManager.TriggerEvent(EnumEvents.SKULLY_HIT_UPPER_RIGHT);
            }
        }
        else if (transform.position == spawnPoint.position)
        {
            // FIX LATER KANSKJE EVENT?
            resetAttackVariables();
        }



        if (target != null)
        {
            MovePointToPoint(transform, target);
        }
    }

    void resetAttackVariables()
    {
        leftToRightSprayFinished = false;
        rightToLeftSprayFinished = false;
        rangedAttacksFinished = false;
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
            base.MakeLoot();
            Destroy(gameObject);
        }
    }
    
    
    public GameObject GetEnemyGameObject()
    {
        return collider.gameObject;
    }
    
    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.SKULLY_HIT_UPPER_LEFT, hitUpperLeftListener);
        EventManager.StartListening(EnumEvents.SKULLY_HIT_UPPER_RIGHT, hitUpperRightListener);
        EventManager.StartListening(EnumEvents.SKULLY_HIT_UPPER_CENTER, hitUpperCenterListener);
        EventManager.StartListening(EnumEvents.SKULLY_HIT_SPAWN_POINT, hitSpawnPointListener);
        DamageBroker.AddToEnemyList(this);

    }

    // Slå av lytter når objektet blir inaktivt (Memory leaks)
    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.SKULLY_HIT_UPPER_LEFT, hitUpperLeftListener);
        EventManager.StopListening(EnumEvents.SKULLY_HIT_UPPER_RIGHT, hitUpperRightListener);
        EventManager.StopListening(EnumEvents.SKULLY_HIT_UPPER_CENTER, hitUpperCenterListener);
        EventManager.StopListening(EnumEvents.SKULLY_HIT_SPAWN_POINT, hitSpawnPointListener);
        DamageBroker.RemoveEnemyFromList(this);
    }
}