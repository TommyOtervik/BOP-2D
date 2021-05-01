using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowCultistScript : Enemy, IDamageable
{
    [SerializeField] private bool drawDebugRaycasts = true;
    [SerializeField] private LayerMask playerLayer;

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float attackDistance; // Min. distance for attack
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer; // Timer for cooldown between attacks

    // RaycastTEst
    [SerializeField] private Transform raycastPoint;

    
    [SerializeField] private float rayCastLength;
    private RaycastHit2D hit;
    
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform rangedAttackPoint;
    

    [SerializeField] private int currentHealth;
    #endregion

    #region Internal Private Variables
    private Animator anim;
    private float distance; // Store distance b/w enemy and player
    private bool inRange; // Check if player is in range
    

    
    #endregion

    //private const int ENEMY_LAYER_INT = 12;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    

    void Update()
    {
        
        HorizontalRayCastCheck();
    }

 
    private void HorizontalRayCastCheck()
    {
        RaycastHit2D playerHit = Raycast(new Vector2(0, -1), Vector2.left, rayCastLength);

        if (playerHit)
            Attack();
        else
        {
            anim.SetTrigger("Idle");
        }
    }

    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, playerLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the player position
        Vector2 pos = transform.position;

        //Send out the desired raycast and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //If we want to show debug raycasts in the scene...
        if (drawDebugRaycasts)
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        //Return the results of the raycast
        return hit;
    }

  

    private void Attack()
    {
        anim.SetTrigger("attackTrigger");
    }

    public void SpawnBolt()
    {
        GameObject bolt = Instantiate(projectile, rangedAttackPoint.position, Quaternion.identity, null);
        bolt.GetComponent<Bolt>().Init(Vector2.left);
    }

    private void StopAttack()
    {
        anim.SetBool("Attack", false);
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
        return null;
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        anim.SetTrigger("Hurt");
        if (currentHealth <= 0)
            Death();
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

