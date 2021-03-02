using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    #region Public varaibles

    [Header("Enemy Info")]
    public int maxHealth = 100;

    public float attackDistance; // Min. distance for attack
    public float moveSpeed;
    public float timer; // Timer for cooldown between attacks

    public Transform leftLimit;
    public Transform rightLimit;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange; // Check if player is in range
    public BoxCollider2D hitBox;
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion


    #region Raycast, bruker "HotZone"
    // public Transform rayCast;
    // public LayerMask raycastMask;
    // public float rayCastLength;
    // private RaycastHit2D hit;
    // private Transfrom target;
    // private bool inRange;
    #endregion


    #region Private Variables 
    Animator anim;

    [SerializeField]
    int currentHealth;

    private float distance; // Store distance b/w enemy and player
    private bool attackMode;

    private bool cooling; // Check if enemy is cooling after attack
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;
    private float intTimer;
    #endregion


    void Awake()
    {
        SelectTarget();

        intTimer = timer;

        anim = GetComponent<Animator>();
       

        currentHealth = maxHealth;

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

        /*  Raycast
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        When player is dectected
        if (hit.collider != null)
            EnemyLogic();
        else if (hit.collider == null)
            inRange = false;
       

        if (inRange == false)
            StopAttack();
        */

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

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", attackMode);
    }

   

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
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
            Die();
        }
    }

    void Die()
    {
        // Die anim
        anim.SetTrigger("Death");


        // Disable the enemy
        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;
        
        this.enabled = false;
    }




    public void TriggerCooling()
    {
        cooling = true;
    }


    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


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

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > target.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }



    /* Brukes for Raycast  
    private void OnTriggerEnter2D(Collider2D collision)
    {
    if (collision.gameObject.CompareTag("Player"))
       {
        target = collision.transform;
        inRange = true;
        Flip();
    }
    }
*/

    /* Raycast Debugger
     private void RaycastDebugger()
    {
     if (distance > attackDistance)
         Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
     else if (attackDistance > distance)
         Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
    }
 */

}
