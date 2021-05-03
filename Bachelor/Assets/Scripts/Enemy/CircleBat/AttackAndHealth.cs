using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAndHealth : Enemy, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
    [SerializeField] private int collisionDamageAmount = 10;
    
    [SerializeField] public float lineOfSight;
    private const int OffsetY = 2;
    private float attackRate = 1.5f;
    
    private const string PLAYER_NAME = "Player";
    private Transform player;
    
    private Collider2D circleBatCollider;
    public GameObject bulletPrefab;
    
    private Animator enemyAnim;

    private float distanceFromPlayer;

    private bool attackInProgress;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        circleBatCollider = GetComponent<BoxCollider2D>();
        enemyAnim = GetComponent<Animator>();
        enemyAnim.SetBool("isDead", false);
        currentHealth = maxHealth;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSight && !attackInProgress)
        {
            StartCoroutine(AttackPattern());
            attackInProgress = true;
        }
    }

    IEnumerator AttackPattern()
    {
        while (true)
        {
            if (distanceFromPlayer > lineOfSight)
            {
                break;
            }

            yield return new WaitForSeconds(attackRate);
            HorizontalVerticalAttack();
            yield return new WaitForSeconds(attackRate);
            DiagonalAttack();
        }

        attackInProgress = false;

    }

    void HorizontalVerticalAttack()
    {
        GameObject tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(Vector2.left);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(Vector2.up);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(Vector2.right);
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(Vector2.down);
        
        
        
    }

    void DiagonalAttack()
    {
        GameObject tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(new Vector2(-1, 1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(new Vector2(1, 1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(new Vector2(-1, -1));
        
        tempBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
        tempBullet.GetComponent<CircleBatBulletMovementAndDamage>().Init(new Vector2(1, -1));
        
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(collisionDamageAmount);
        }
        
        Physics2D.IgnoreCollision(circleBatCollider, collision.collider, true);

    }
    
    
    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        enemyAnim.SetTrigger("wasHit");
        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        if (gameObject != null)
        {
            base.MakeLoot();
            enemyAnim.SetBool("isDead", true);
            Destroy(gameObject);
        }
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
        return circleBatCollider.gameObject;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
