using System;
using UnityEngine;

public class JumpEnemyAttacker : Enemy, IDamageable
{
    [Header("For Patrolling")]
    [SerializeField] private float moveSpeed;
    private float moveDirection = 1;
    private bool facingRight = true;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask groundLayer;
    private bool checkingGround;
    private bool checkingWall;

    [Header("For Jumpattack")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private Transform player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize;
    private bool isGrounded;

    [Header("For seeing player")] 
    [SerializeField] private Vector2 lineOfSight;
    [SerializeField] private LayerMask playerLayer;
    private bool canSeePlayer;
    
    [Header("Other")]
    private Rigidbody2D enemyRb;
    private Animator enemyAnim;
    private int health;
    private int damageAmount = 10;
    private const string PLAYER_NAME = "Player";


    
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int currentHealth;
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;

    private bool isDead;

    private Collider2D porkuCollider;
    
    
    private void MakeLoot()
    {
        base.MakeLoot();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();

        porkuCollider = GetComponent<BoxCollider2D>();
        //enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheckPoint.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheckPoint.position, circleRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(transform.position, lineOfSight, 0, playerLayer);

        AnimationController();
        
        if (!canSeePlayer && isGrounded)
        {
            Patrolling();
        }   
        
    }

    // FIXME: IKKE BRA. 
    private void Update()
    {
        if (isGrounded && isDead)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            porkuCollider.enabled = false;
            this.enabled = false;
        }
    }

    void Patrolling()
    {
        
        if (!checkingGround || checkingWall)
        {
            Flip();
        }
        
        enemyRb.velocity = new Vector2(moveSpeed * moveDirection, enemyRb.velocity.y);
    }

    void JumpAttack()
    {
        float distanceFromPlayer = player.position.x - transform.position.x;

        if (isGrounded)
        {
            enemyRb.AddForce(new Vector2(distanceFromPlayer, jumpHeight), ForceMode2D.Impulse);
        }
    }

    void FlipTowardsPlayer()
    {
        float playerPosition = player.position.x - transform.position.x;
        if (playerPosition < 0 && facingRight)
        {
            Flip();
        } 
        else if (playerPosition > 0 && !facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    void AnimationController()
    {
        enemyAnim.SetBool("canSeePlayer", canSeePlayer);
        enemyAnim.SetBool("isGrounded", isGrounded);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckPoint.position, circleRadius);
        Gizmos.DrawWireSphere(wallCheckPoint.position, circleRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawCube(groundCheck.position, boxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, lineOfSight);
        
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        // FIXME: == 12, Equals?
        if (collision.gameObject.layer == 12)
        {
            Physics2D.IgnoreCollision(porkuCollider, collision.collider, true);
        }


        if (collision.gameObject.name.Equals(PLAYER_NAME))
        {
            DamageBroker.CallTakeDamageEvent(damageAmount);
            Physics2D.IgnoreCollision(porkuCollider, collision.collider, true);
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



    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        int hurtRand = UnityEngine.Random.Range(minRandomHurt, maxRandomHurt + 1);
        if (hurtRand == 1)
        {
            enemyAnim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {

        enemyAnim.SetTrigger("Death");
        MakeLoot();
        isDead = true;
       
    }

    public GameObject GetEnemyGameObject()
    {
        return porkuCollider.gameObject;
    }
}
