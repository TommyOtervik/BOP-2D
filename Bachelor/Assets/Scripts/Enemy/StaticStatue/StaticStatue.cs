using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticStatue : Enemy, IDamageable
{
    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    public LayerMask groundLayer;

    private bool canAttackHorizontal;
    private bool canAttackGrenade;
    private bool horizontalAttackInProgress;
    private bool grenadeAttackInProgress;
    BoxCollider2D bodyCollider;
    
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;

    [SerializeField] Transform horizontalAttackPoint;
    [SerializeField] private Transform grenadeLaunchAttackPoint;

    private const string PLAYER_NAME = "Player";
    private int maxHealth = 100;
    private int currentHealth;
    public int collisionDamageAmount = 10;

        void Start()
    {
        bodyCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        HorizontalRayCastCheck();
        GrenadeRayCastCheck();
        if (canAttackHorizontal && !canAttackGrenade)
        {
            if (!horizontalAttackInProgress && !grenadeAttackInProgress)
            {
                StartCoroutine(HorizontalAttackPattern());
            }
        }

        if (canAttackGrenade)
        {
            if (!horizontalAttackInProgress && !grenadeAttackInProgress)
            {
                StartCoroutine(GrenadeAttackPattern());
            }
        }

    }

    void HorizontalRayCastCheck()
    {
        RaycastHit2D playerHit = Raycast(new Vector2(0, -1), Vector2.left, 10);
        if (playerHit)
        {
            canAttackHorizontal = true;
        }
        else
        {
            canAttackHorizontal = false;
        }
    }
    
    void GrenadeRayCastCheck()
    {
        RaycastHit2D playerHit = Raycast(new Vector2(0, 1) * -1.3f, Vector2.left, 3);
        if (playerHit)
        {
            canAttackGrenade = true;
        }
        else
        {
            canAttackGrenade = false;
        }
    }
    
    void HorizontalAttack()
    { 
        Instantiate(bulletPrefab, horizontalAttackPoint.position, Quaternion.identity, null);
    }

    void GrenadeAttack()
    {
        GameObject tempGrenade = Instantiate(grenadePrefab, grenadeLaunchAttackPoint.position, Quaternion.identity, null);
        tempGrenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1), ForceMode2D.Impulse);
    }

    IEnumerator HorizontalAttackPattern()
    {
        horizontalAttackInProgress = true;
        HorizontalAttack();
        yield return new WaitForSeconds(0.5f);
        HorizontalAttack();
        yield return new WaitForSeconds(0.5f);
        HorizontalAttack();
        yield return new WaitForSeconds(0.5f);
        HorizontalAttack();
        yield return new WaitForSeconds(0.5f);
        HorizontalAttack();
        yield return new WaitForSeconds(2.0f);
        horizontalAttackInProgress = false;
    }
    
    IEnumerator GrenadeAttackPattern()
    {
        grenadeAttackInProgress = true;
        GrenadeAttack();
        yield return new WaitForSeconds(0.5f);
        GrenadeAttack();
        yield return new WaitForSeconds(0.5f);
        GrenadeAttack();
        yield return new WaitForSeconds(0.5f);
        GrenadeAttack();
        yield return new WaitForSeconds(0.5f);
        GrenadeAttack();
        yield return new WaitForSeconds(1.0f);
        grenadeAttackInProgress = false;
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
    
    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        
        if (currentHealth <= 0)
            Death();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.name.Equals(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(collisionDamageAmount);
        }

    }
    
    public void Death()
    {
        base.MakeLoot();
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        DamageBroker.AddToEnemyList(this);
    }

    private void OnDisable()
    {
        DamageBroker.RemoveEnemyFromList(this);
    }
    
    public GameObject GetEnemyGameObject()
    {
        return bodyCollider.gameObject;
    }
}
    

