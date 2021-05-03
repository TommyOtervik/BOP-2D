using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

public class Skully : Enemy, IDamageable
{
    // Collision enter etterpå
    private int maxHealth = 30;
    private int currentHealth;
    private Collider2D collider;
    private float speed = 10.0f;

    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointLeft;
    [SerializeField] private Transform attackPointRight;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform upperLeft;
    [SerializeField] private Transform upperRight;
    [SerializeField] private Transform upperCenter;

    [SerializeField] private GameObject bulletPrefab;
    private float attackRate = 0.4f;
    private Transform positionTarget;
    private bool hasTarget;
    
    private bool attackInProgress = false;
    private bool leftToRightSprayFinished = false;
    private bool rightToLeftSprayFinished = false;


    private int downAttackBulletAmount = 9;
    private int sideAttackBulletAmount = 5;

    private MovementState movementState;
    private AttackState attackState;
    private SkullBossSpawner spawner;
    
    

    

// Start is called before the first frame update
    void Awake()
    {
        
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        spawner = GetComponent<SkullBossSpawner>();
        currentHealth = maxHealth;
        transform.position = spawnPoint.position;
        //positionTarget = upperLeft;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void PickTarget()
    {
        
        if (!hasTarget)
        {
            int attackNumber = UnityEngine.Random.Range(0,2);
            switch(attackNumber) 
            {
                case 0:
                    // UpperLeft
                    positionTarget = upperLeft;
                    hasTarget = true;
                    break;
                case 1:
                    // UpperRight
                    positionTarget = upperRight;
                    hasTarget = true;
                    break;
                default:
                    // code block
                    break;
            }    
        }
        
    }
    

    void PickAttack()
    {
        
        int attackNumber = UnityEngine.Random.Range(0,3);
        switch(attackNumber) 
        {
            case 0:
                // Attack1 
                break;
            case 1:
                // Attack2
                break;
            case 2:
                // Attack3
                break;
            default:
                // code block
                break;
        }
    }
    
    void AirAttack()
    {
        StartCoroutine(SpawnBulletsDown(downAttackBulletAmount, attackRate));
    }

    void SideAttackLeft()
    {
        spawner.SpawnBulletsFromLeft(sideAttackBulletAmount);
    }

    void SideAttackRight()
    {
        spawner.SpawnBulletsFromRight(sideAttackBulletAmount);
    }

    void AngleAttackLeft()
    {
        StartCoroutine(SpawnBulletsLeftAngle(17, attackRate));
    }

    void AngleAttackRight()
    {
        StartCoroutine(SpawnBulletsRightAngle(17, attackRate));
    }

    IEnumerator SpawnBulletsDown(int amount, float delay)
    {
        
        
        GameObject tempBullet;
        for (int i = 0; i < amount; i++)
        {
            tempBullet = Instantiate(bulletPrefab, attackPointDown.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().Init(Vector2.down);

            yield return new WaitForSeconds(delay);

        }

        
        
    }
    
    IEnumerator SpawnBulletsLeftAngle(int amount, float delay)
    {
        
        Vector2 startVector;
        float initialX = attackPointLeft.position.x - 7;
        float initialY = attackPointLeft.position.y + 7;

        float x = initialX;
        float y = initialY;
        // For hver iterasjon x positiv, y negativ
        int waveCounter = 0;
        int waveLimit = 10;
        
        GameObject tempBullet;
        
        for (int j = 0; waveCounter < waveLimit; j++) {
            
            for (int i = 0; i < amount; i++)
            {
                if (i % 2 == 0)
                {
                    startVector = new Vector2(x, y);
                    tempBullet = Instantiate(bulletPrefab, startVector, Quaternion.identity);
                    tempBullet.GetComponent<Bullet>().Init(new Vector2(-1, -1));
                }
                
                x++;
                y--;
            
            }
            waveCounter++;
            x = initialX;
            y = initialY;
            
            yield return new WaitForSeconds(0.5f);

            
        }
        
    }
    
    IEnumerator SpawnBulletsRightAngle(int amount, float delay)
    {
        
        Vector2 startVector;
        float initialX = attackPointRight.position.x - 7;
        float initialY = attackPointRight.position.y - 7;

        float x = initialX;
        float y = initialY;
        // For hver iterasjon x positiv, y negativ
        int waveCounter = 0;
        int waveLimit = 10;
        
        GameObject tempBullet;
        
        for (int j = 0; waveCounter < waveLimit; j++) {
            
            for (int i = 0; i < amount; i++)
            {
                if (i % 2 == 0)
                {
                    startVector = new Vector2(x, y);
                    tempBullet = Instantiate(bulletPrefab, startVector, Quaternion.identity);
                    tempBullet.GetComponent<Bullet>().Init(new Vector2(1, -1));
                }
                
                x++;
                y++;
            
            }
            waveCounter++;
            x = initialX;
            y = initialY;
            
            yield return new WaitForSeconds(0.5f);
        }

        


    }
    

    // Beveg skully mellom 2 punkter
    void MovePointToPoint(Transform current, Transform target)
    {
        if (current.position != target.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        



    }

    void MovementCheck()
    {
        if (hasTarget)
        {
            MovePointToPoint(transform, positionTarget);
        }
        
    }

    
    

    void WaitInSeconds(float seconds)
    {
        StartCoroutine(WaitForSeconds(seconds));
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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
        DamageBroker.AddToEnemyList(this);

    }

    // Slå av lytter når objektet blir inaktivt (Memory leaks)
    private void OnDisable()
    {
       
        DamageBroker.RemoveEnemyFromList(this);
    }
}
