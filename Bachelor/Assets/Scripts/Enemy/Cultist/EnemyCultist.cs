using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyCultist : Enemy, IDamageable
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

    private Collider2D cultistCollider;
    #endregion

    #region Events
    private UnityAction playerDeadListener;
    #endregion


    void Awake()
    {

        SelectTarget();

        intTimer = timer;
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        playerDeadListener = new UnityAction(StopAttack);


        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
        {
            if (c.name.Equals("EnemyColliders"))
                cultistCollider = c;
        }

    }
    
    public GameObject GetEnemyGameObject()
    {
        return cultistCollider.gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        // FIXME: == 12, Equals?
        if (collision.gameObject.layer == 12 && collision.collider.name.Equals("EnemyColliders"))
        {
            Physics2D.IgnoreCollision(cultistCollider, collision.collider, true);
        }
    }

    void Update()
    {
        if (!attackMode)
            Move();

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
            SelectTarget();
        

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


    // Håndterer skade som er gjort mot AI.
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        int hurtRand = UnityEngine.Random.Range(minRandomHurt, maxRandomHurt + 1);
        if (hurtRand == 1)
        {
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            MakeLoot();
            Death();
        }

        
        
    }
    
    protected override void MakeLoot()
    {
        if (thisLoot != null)
        {
            Pickup current = thisLoot.LootPickup();
            if (current != null)
            {
                Instantiate(current.gameObject, new Vector2(transform.position.x + 0.5f, transform.position.y + 3), Quaternion.identity);
            }
        }
    }

    // Håndterer død av AI
    public void Death()
    {
        // Die anim
        anim.SetTrigger("Death");
        attackMode = false;
        anim.SetBool("Attack", attackMode);

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        // Disable the enemy
        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;

        this.enabled = false;

        EventManager.TriggerEvent(EnumEvents.CULTIST_DEAD);
    }

    // Brukes i animator for å vente med å angripe etter et slag.
    public void TriggerCooling()
    {
        cooling = true;
    }


    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


    // Velger "Patruljeringspunkt" ut i fra distansen,
    // velger den som er lengst unna.
    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);


        if (distanceToLeft > distanceToRight)
            target = leftLimit;
        else
            target = rightLimit;

        Flip();
    }


    // Snur fienden
    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > target.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }

    // Subscriber til events
    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.PLAYER_DEAD, playerDeadListener);

        DamageBroker.AddToEnemyList(this);
    }

    // Unsubscriber til events
    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.PLAYER_DEAD, playerDeadListener);

        DamageBroker.RemoveEnemyFromList(this);
    }


    // Setters som brukes i TriggerAreaCheck.cs
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    public void SetHotZone(bool hotZone)
    {
        this.hotZone.SetActive(hotZone);
    }

    public void SetTriggerArea(bool triggerArea)
    {
        this.triggerArea.SetActive(triggerArea);
    }

    public Animator GetAnimator()
    {
        return this.anim;
    }
}
