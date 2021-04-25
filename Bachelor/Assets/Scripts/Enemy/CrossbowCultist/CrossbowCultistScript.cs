using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowCultistScript : MonoBehaviour, IDamageable
{
    [SerializeField] private bool drawDebugRaycasts = true;
    [SerializeField] private LayerMask groundLayer;

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
    //[SerializeField] private Transform leftLimit;
    //[SerializeField] private Transform rightLimit;

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
        //SelectTarget(); 

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
        HorizontalRayCastCheck();
    }

 
    private void HorizontalRayCastCheck()
    {
        RaycastHit2D playerHit = Raycast(new Vector2(0, -1), Vector2.left, rayCastLength);

        if (playerHit)
            Attack();
    }

    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, groundLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the enemy's position
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

