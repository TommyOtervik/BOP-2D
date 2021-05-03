using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Dette skriptet tilhører CrossbowCultist. En statisk fiende som baserer
 * angrep på RayCast.
 * 
 */
public class CrossbowCultistScript : Enemy, IDamageable
{
    [SerializeField] private bool drawDebugRaycasts = true; // Debug
    
    [SerializeField] private LayerMask playerLayer;         // Spillerens "lag" (Layer)

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;  // Maksimal helse
    [SerializeField] private float attackDistance; // Min. distanse for angrep
    
    [SerializeField] private Transform raycastPoint; // Punkt for RayCast
    [SerializeField] private float rayCastLength;    // RayCast lengde
    
    [SerializeField] private GameObject projectile;       // Objektet for pil (Bolt) som skytes
    [SerializeField] private Transform rangedAttackPoint; // Angreps punkt
    
    [SerializeField] private int currentHealth; // Nåværende helse
    #endregion

    #region Internal Private Variables
    private Animator anim; // Animator referanse 
    #endregion

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

        // Sjekker om RayCast treffer spilleren, gjør den det -> Angrip
        if (playerHit)
            Attack();
        else
            anim.SetTrigger("Idle");
        
    }

    // Angreps metode, setter bare animasjon siden den håndterer instantiering av pilene.
    private void Attack()
    {
        anim.SetTrigger("attackTrigger");
    }

    // Brukes i animasjonen
    public void SpawnBolt()
    {
        GameObject bolt = Instantiate(projectile, rangedAttackPoint.position, Quaternion.identity, null);
        bolt.GetComponent<Bolt>().Init(Vector2.left);
    }
    
    // Metode som håndterer død
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
        return this.gameObject;
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

}

