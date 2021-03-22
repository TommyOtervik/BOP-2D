using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowCultistScript : MonoBehaviour, IDamageable
{

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float attackDistance; // Min. distance for attack
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer; // Timer for cooldown between attacks

    // RaycastTEst
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float rayCastLength;
    private RaycastHit2D hit;
    [SerializeField] private Transform target;

    // Venstre & Høyre grense for patruljering
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    [SerializeField] private GameObject projectile;
    [SerializeField] private Vector3 projectionSpawnOffset;
    

    [SerializeField] private int currentHealth;
    #endregion

    #region Internal Private Variables
    private Animator anim;
    private float distance; // Store distance b/w enemy and player
    private bool attackMode;
    private bool inRange; // Check if player is in range
    private bool cooling;
    private int intTimer;

    private Collider2D cultistCollider;
    #endregion

    //private const int ENEMY_LAYER_INT = 12;


    void Awake()
    {
        SelectTarget(); 

        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
        {
            if (c.name.Equals("EnemyColliders"))
                cultistCollider = c;
        }
    }

    void Update()
    {
        

        if (!attackMode)
            Move();

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
            SelectTarget();

        if(inRange)
        {
            hit = Physics2D.Raycast(raycastPoint.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        if (hit.collider != null)
            EnemyLogic();
        else if (hit.collider == null)
            inRange = false;

        if (!inRange)
            StopAttack();
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

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > target.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }

    private void RaycastDebugger()
    {
        if (distance > attackDistance)
            Debug.DrawRay(raycastPoint.position, transform.right * rayCastLength, Color.red);
        else if (attackDistance > distance)
            Debug.DrawRay(raycastPoint.position, transform.right * rayCastLength, Color.green);
    }

    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && !cooling)
        {
            Attack();
        }

        if (cooling)
        {
            anim.SetBool("Attack", true);
        }

    }
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.transform;
            inRange = true;
            Flip();
        }
    }

    private void Move()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("CrossbowCultist_attack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }
   
    public void SpawnBolt()
    {
      
    }

   
    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetEnemyGameObject()
    {
        return cultistCollider.gameObject;
    }

    public void TakeDamage(int damageTaken)
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        DamageBroker.AddToEnemyList(this);
    }

    // Unsubscriber til events
    private void OnDisable()
    {
        DamageBroker.RemoveEnemyFromList(this);
    }

}

