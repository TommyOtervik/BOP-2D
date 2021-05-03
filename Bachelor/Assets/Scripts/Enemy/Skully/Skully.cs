using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

/*
 *  
 */
public class Skully : Enemy, IDamageable
{
   
    private int maxHealth = 1000;
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
    [SerializeField] private Transform upperLeftForAirAttack;
    [SerializeField] private Transform upperRightForAirAttack;

    [SerializeField] private GameObject bulletPrefab;
    private float attackRate = 0.4f;
    private Transform target;

    private bool attackInProgress = false;
    
    private bool leftToRightSprayFinished = false;
    private bool rightToLeftSprayFinished = false;


    private int downAttackBulletAmount = 9;
    private int sideAttackBulletAmount = 5;
    
    [SerializeField] private SkullBossSpawner spawner;
    
    private bool sleepMode;
    
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite redSprite;
    
    [SerializeField] private SkullyHealthBar healthBar;

    





// Start is called before the first frame update
    void Awake()
    {
        attackInProgress = false;
        sleepMode = false;
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        healthBar.SetSize((float)currentHealth / (float)maxHealth);
        target = spawnPoint;


    }

    // Update is called once per frame
    void Update()
    {
        MovePointToPoint(transform, target);
    }
    
    // Beveg skully mellom 2 punkter
    void MovePointToPoint(Transform current, Transform target)
    {
        

        if (current.position == target.position)
        {
             
            if (current.position == spawnPoint.position)
            {
                if (sleepMode)
                {
                    return;
                }
                // Venter i 2 sekunder før han velger target
                PickTargetWaitInSeconds(2);
                
                
            }
            else if (current.position == upperLeft.position && !attackInProgress)
            {
                AngleAttackRight();
            }
            else if (current.position == upperRight.position && !attackInProgress)
            {
                AngleAttackLeft();

            } else if (current.position == upperRightForAirAttack.position && !attackInProgress)
            {
                AirAttack("Left");
                SideAttackLeft();
            } else if (current.position == upperLeftForAirAttack.position && !attackInProgress)
            {
                AirAttack("Right");
                SideAttackRight();
            }


        }
        
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    
    void PickTargetWaitInSeconds(int seconds)
    {
        StartCoroutine(WaitForSeconds(seconds));
    }

    IEnumerator WaitForSeconds(int seconds)
    {
        sleepMode = true;
        yield return new WaitForSeconds(seconds);
        PickTarget();
        sleepMode = false;
    }

    void ChangeSpriteToRed()
    {
        spriteRenderer.sprite = redSprite;
    }

    void ChangeSpriteToNormal()
    {
        spriteRenderer.sprite = normalSprite;
    }

    // Velger target basert på 4 tomme objekter i scenen
    void PickTarget()
    {
        {
            int attackNumber = UnityEngine.Random.Range(0, 4);
            switch (attackNumber)
            {
                case 0:
                    // UpperLeft
                    target = upperLeft;
                    break;
                case 1:
                    // UpperRight
                    target = upperRight;
                    break;
                case 2: // UpperLeftAir
                    target = upperLeftForAirAttack;
                    break;
                case 3: // UpperRightAir
                    target = upperRightForAirAttack;
                    break;
                default:
                    // code block
                    break;
            }
        }
    }
    
    
    void AirAttack(string direction)
    {
        StartCoroutine(SpawnBulletsDown(downAttackBulletAmount, attackRate, direction));
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
    // Air attack
    IEnumerator SpawnBulletsDown(int amount, float delay, string direction)
    {
        if (direction.Equals("Left"))
        {
            target = upperLeftForAirAttack;
        }
        else
        {
            target = upperRightForAirAttack;
        }
        

        attackInProgress = true;
        
        GameObject tempBullet;
        for (int i = 0; i < amount; i++)
        {
            tempBullet = Instantiate(bulletPrefab, attackPointDown.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().Init(Vector2.down);

            yield return new WaitForSeconds(delay);

        }
        
        attackInProgress = false; 
        target = spawnPoint;
        




    }
    // Skrått attack 1
    IEnumerator SpawnBulletsLeftAngle(int amount, float delay)
    {
        ChangeSpriteToRed();
        attackInProgress = true;
        Vector2 startVector;
        float initialX = attackPointLeft.position.x - 7;
        float initialY = attackPointLeft.position.y + 7;

        float x = initialX;
        float y = initialY;
        
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

        attackInProgress = false;
        target = spawnPoint;
        ChangeSpriteToNormal();

    }
    // Skrått attack 2
    IEnumerator SpawnBulletsRightAngle(int amount, float delay)
    {
        ChangeSpriteToRed();
        attackInProgress = true;
        Vector2 startVector;
        float initialX = attackPointRight.position.x - 7;
        float initialY = attackPointRight.position.y - 7;

        float x = initialX;
        float y = initialY;
        
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

        attackInProgress = false;
        target = spawnPoint;
        ChangeSpriteToNormal();

    }
    
    
    

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        healthBar.SetSize((float)currentHealth / (float)maxHealth);
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
