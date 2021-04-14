using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbominationMiniBoss : MonoBehaviour, IDamageable
{

    #region External Private Variables
    [Header("Abomination Info")]
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;
    [SerializeField] private float timer;
    [SerializeField] private bool inRange;
    [SerializeField] private Transform player;

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
    private bool insideHotZone;
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;
    private float intTimer;
    private Collider2D abomCollider;
    #endregion

 

    private UnityAction playerDeadListener;
    // Start is called before the first frame update
    void Awake()
    {
        // transform.Rotate(0f, 0f, 0f);

        playerDeadListener = new UnityAction(StopAttack);

        anim = GetComponent<Animator>();
        abomCollider = GetComponent<BoxCollider2D>();

        currentHealth = maxHealth;

        anim.SetBool("canWalk", false);
    }

    // Update is called once per frame
    void Update()
    {
       

        if (!attackMode && insideHotZone)
            Move();
        else // Reset?
            anim.SetBool("canWalk", false);



        if (inRange)
            EnemyLogic();

        
    }

    private void Move()
    {
        anim.SetBool("canWalk", true);
        
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Abomination_Attack"))
        {
            Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPos) <= attackRange)
                inRange = true;
            else
                inRange = false;


            Flip();
        }
    }

    private void EnemyLogic()
    {
        
        distance = Vector2.Distance(transform.position, player.position);
       
        if (distance > attackRange)
            StopAttack();
        else if (distance <= attackRange && !cooling)
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
        Debug.Log(timer);
        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    private void Attack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("canWalk", !attackMode);
        anim.SetBool("Attack", attackMode);
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(abomCollider, collision.collider, true);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        int hurtRand = UnityEngine.Random.Range(minRandomHurt, maxRandomHurt + 1);
        if (hurtRand == 1)
        {
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
            Death();
    }

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
    }

    // Brukes i animator for å vente med å angripe etter et slag.
    public void TriggerCooling()
    {
        cooling = true;
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > player.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.PLAYER_DEAD, playerDeadListener);
        DamageBroker.AddToEnemyList(this);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.PLAYER_DEAD, playerDeadListener);
        DamageBroker.RemoveEnemyFromList(this);
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

    public void SetInsideHotZone(bool b)
    {
        this.insideHotZone = b;
    }

    public GameObject GetEnemyGameObject()
    {
        return abomCollider.gameObject;
    }
}
