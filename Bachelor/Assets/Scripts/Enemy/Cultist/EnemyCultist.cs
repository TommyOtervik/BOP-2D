using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyCultist : MonoBehaviour, IDamageable<int>
{

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float attackDistance; // Min. distance for attack
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer; // Timer for cooldown between attacks
    [SerializeField] private bool inRange; // Check if player is in range
    [SerializeField] private Transform target;

    // Venstre & Høyre grense for patruljering
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    // Søke / trigger område for AI
    [SerializeField] private GameObject hotZone;
    [SerializeField] private GameObject triggerArea;

    // BoxCollider for våpen til fiende
    [SerializeField] private BoxCollider2D hitBox;
    
    [SerializeField] private int currentHealth;
    #endregion


    #region Internal Private Variables
    private Animator anim;
    private float distance; // Store distance b/w enemy and player
    private bool attackMode;

    private bool cooling; // Check if enemy is cooling after attack
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;
    private float intTimer;
    #endregion

    #region Events
    private UnityAction flipCultistListener;
    private const string FLIP_CULTIST_KEY = "FlipCultist";
    
    private UnityAction hotZoneExitListener;
    private const string HOT_ZONE_EXIT_KEY = "HotZoneExit";

    #endregion


    void Awake()
    {
        SelectTarget();

        intTimer = timer;

        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        flipCultistListener = new UnityAction(Flip);
        hotZoneExitListener = new UnityAction(HotZoneExit);
  


        // Test 
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
            EnemyLogic();

    }


    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
            StopAttack();
        else if (distance <= attackDistance && !cooling)
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

        anim.SetBool("canWalk", !attackMode);
        anim.SetBool("Attack", attackMode);

    }

    public void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
    }

    public void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPos = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt anim
        //if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) { 
        //    anim.SetTrigger("Hurt");
        //}
        int hurtRand = UnityEngine.Random.Range(minRandomHurt, maxRandomHurt + 1);
        if (hurtRand == 1)
        {
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        // Die anim
        anim.SetTrigger("Death");
        attackMode = false;
        anim.SetBool("Attack", attackMode);

        // Disable the enemy
        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;
        
        this.enabled = false;
    }



    // Bruker i animator for å sette "Cooldown" etter slag.
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

    private void HotZoneExit()
    {
        
        triggerArea.SetActive(true);
        inRange = false;
        SelectTarget();
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

    private void OnEnable()
    {
        EventManager.StartListening(FLIP_CULTIST_KEY, flipCultistListener);
        EventManager.StartListening(HOT_ZONE_EXIT_KEY, hotZoneExitListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening(FLIP_CULTIST_KEY, flipCultistListener);
        EventManager.StopListening(HOT_ZONE_EXIT_KEY, hotZoneExitListener);

    }

    // Getters / Setters
    public bool GetAttackmode()
    {
        return attackMode;
    }

  

    public GameObject GetHotZone()
    {
        return hotZone;
    }

    public void SetInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

}
