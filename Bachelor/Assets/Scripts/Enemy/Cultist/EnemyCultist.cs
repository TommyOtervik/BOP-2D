using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Dette skriptet tilhører Cultist.
 * Holder styr på helse, angrep, animasjoner og patruljering 
 * 
 * @AOP - 225280
 */
public class EnemyCultist : Enemy, IDamageable
{

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;  // Max. helse
    [SerializeField] private float attackDistance; // Min. distanse for angrep
    [SerializeField] private float moveSpeed;      // Hastighet
    [SerializeField] private float timer;          // Nedteller for tiden mellom angrep
    [SerializeField] private bool inRange;         // Sjekker om spilleren er innenfor rekkevidde
    [SerializeField] private Transform target;     // "Mål" / Spilleren

    
    [SerializeField] private Transform leftLimit;  // Venstre grense for patruljering
    [SerializeField] private Transform rightLimit; // Høyre grense for patruljering

    [SerializeField] private GameObject hotZone;      // Søke område
    [SerializeField] private GameObject triggerArea;  // Trigger område
    [SerializeField] private BoxCollider2D hitBox;    // BoxCollider for våpen

    [SerializeField] private int currentHealth;       // Nåværende helse
    #endregion


    #region Internal Private Variables
    private Animator anim;   // Animator referanse
    private float distance;  // Distanse mellom fiende og spiller
    private bool attackMode; // Angreps modus

    private bool cooling;               // Sjekker om fiende "kjøler ned" etter angrep
    private int MIN_RANDOM_HURT = 1;    // Brukes til animajson, gir 10% sjanse til å sette Abom. i "Hurt" anim.
    private int MAX_RANDOM_HURT = 10;
    private float intTimer;
    private const int PLAYER_LAYER_INT = 12;

    private Collider2D cultistCollider; 
    #endregion

    #region Events
    private UnityAction playerDeadListener;
    #endregion


    void Awake()
    {
        // Velger patruljerings-punk
        SelectTarget();

        intTimer = timer;
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        // Lytter om spilleren er død
        playerDeadListener = new UnityAction(StopAttack);

        // Henter komponenter
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
        // Ignorerer kollisjon med spiller og andre fiender
        if (collision.gameObject.layer == PLAYER_LAYER_INT && collision.collider.name.Equals("EnemyColliders"))
            Physics2D.IgnoreCollision(cultistCollider, collision.collider, true);
        
    }

    void Update()
    {
        // Så lenge han ikke angriper -> Gå
        if (!attackMode)
            Move();

        // Hvis spilleren ikke er i nærheten -> Patruljer
        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
            SelectTarget();
        
        // Er spilleren i nærheten
        if (inRange)
            EnemyLogic();
    }


    // Fiende logikk
    private void EnemyLogic()
    {
        // Distanse mellom fiende og spiller
        distance = Vector2.Distance(transform.position, target.position);

        
        if (distance > attackDistance) // Er spilleren for langt unna -> Stop å angripe
            StopAttack();
        else if (distance <= attackDistance && !cooling) // Er spilleren innenfor rekkevidde -> Angrip
            Attack();

        // "Kjøler ned", venter mellom angrep
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
        timer = intTimer;  // Restart tidtaker når spilleren kommer innenfor rekkevidde
        attackMode = true; // For å sjekke om fienden kan angripe eller ikke

        anim.SetBool("canWalk", !attackMode); // "Stopper" fienden (animasjon)
        anim.SetBool("Attack", attackMode);   // Angreps animasjon
    }

    public void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
    }

    public void Move()
    {
        anim.SetBool("canWalk", true); // Gå animasjon 

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }


    // Håndterer skade
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        int hurtRand = UnityEngine.Random.Range(MIN_RANDOM_HURT, MAX_RANDOM_HURT + 1);
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

    // Håndterer død 
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


    // Setters
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
