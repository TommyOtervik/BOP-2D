using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    private float speed = 4f;                //Player speed
    private float coyoteDuration = .05f;     //How long the player can jump after falling
    private float maxFallSpeed = -25f;       //Max speed player can fall


    // Testing for bedre hopping
    [Range(1, 10)]
    public float jumpVelocity;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;
    // End Bedre hopp

    // Testing for angrep
    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField]
    private float attackRange = 1f;
    private int attackDamage = 20;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;
    private bool isEnemy = false;
    // End Test Angrep


    [Header("Environment Check Properties")]
    public float footOffset = .32f;          //X Offset of feet raycast
    public float eyeHeight = 1.11f;          //Height of wall checks
    public float headClearance = .25f;       //Space needed above the player's head
    public float groundDistance = .35f;      //Distance player is considered to be on the ground
    public float grabDistance = .4f;        //The reach distance for wall grabs
    public LayerMask groundLayer;           //Layer of the ground
    

    private bool isOnGround = false;         //Is the player on the ground?
    private bool isJumping;                  //Is player jumping?
    private bool isHeadBlocked;



    PlayerInput input;                      //The current inputs for the player
    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component
    Animator anim;
    PlayerMovement movement;


    [SerializeField] GameObject projectile;
    [SerializeField] Vector3 projectionSpawnOffset;

    float delayToIdle = 0.0f;
    float jumpTime;                         //Variable to hold jump duration
    float coyoteTime;                       //Variable to hold coyote duration
    float playerHeight;                     //Height of the player

    float originalXScale;                   //Original scale on X axis
    int direction = 1;                      //Direction player is facing

    Vector2 colliderStandSize;              //Size of the standing collider
    Vector2 colliderStandOffset;			//Offset of the standing collider

    const float smallAmount = .05f;			//A small amount used for hanging position

    // Start is called before the first frame update
    void Start()
    {
        //Get a reference to the required components
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        movement = GetComponent<PlayerMovement>();

        //If any of the needed components don't exist...
        if (movement == null || rigidBody == null || input == null || anim == null)
        {
            //...log an error and then remove this component
            Debug.LogError("A needed component is missing from the player");
            Destroy(this);
        }

        //Record the original x scale of the player
        originalXScale = transform.localScale.x;

        //Record the player's height from the collider
        playerHeight = bodyCollider.size.y;

        //Record initial collider size and offset
        colliderStandSize = bodyCollider.size;
        colliderStandOffset = bodyCollider.offset;
    }

    void FixedUpdate()
    {
        //Check the environment to determine status
        PhysicsCheck();

        //Process ground and air movements
        GroundMovement();
        MidAirMovement();
        AttackManager();
    }

    void PhysicsCheck()
    {
        //Start by assuming the player isn't on the ground and the head isn't blocked
        isOnGround = false;
        isHeadBlocked = false;

        //Cast rays for the left and right foot
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, .2f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, .2f), Vector2.down, groundDistance);

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
            anim.SetBool("Grounded", isOnGround);
        }
        else if (!leftCheck && !rightCheck)
        {
            isOnGround = false;
            anim.SetBool("Grounded", isOnGround);
        }

        //Cast the ray to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

        //If that ray hits, the player's head is blocked
        if (headCheck)
            isHeadBlocked = true;
    }

    void GroundMovement()
    {
        //Calculate the desired velocity based on inputs
        float inputX = Input.GetAxis("Horizontal");
        float xVelocity = speed * inputX;
        //If the sign of the velocity and direction don't match, flip the character
        if (xVelocity * direction < 0f)
            FlipCharacterDirection();

        //Apply the desired velocity 
        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);


        //If the player is on the ground, extend the coyote time window
        if (isOnGround)
        {
            coyoteTime = Time.time + coyoteDuration;
        }

        // Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            delayToIdle = 0.05f;
            anim.SetInteger("AnimState", 1);
        }
        // Idle
        else
        {
            // Prevents flickering transitions to idle
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                anim.SetInteger("AnimState", 0);
        }


    }
    void MidAirMovement()
    {

        anim.SetFloat("AirSpeedY", rigidBody.velocity.y);

        if ((input.jumpHeld || input.jumpPressed) && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {
            isOnGround = false;
            anim.SetTrigger("Jump");
            isJumping = true;

            rigidBody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }


        if (rigidBody.velocity.y < 0)
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidBody.velocity.y > 0 && !input.jumpHeld)
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


        if (isOnGround)
            isJumping = false;



        //If player is falling to fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);

    }

    void AttackManager()
    {

        if(Time.time >= nextAttackTime)
        {
            if (input.firePressed)
            {
                MeleeAttack();
                // If attack rate is 2, add 1 divided by 2 = 0.5 sec 
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }

       
        if (input.altFirePressed)
        {
            AltMeleeAttack();
        }
        else if (input.rangedAttack)
        {
            RangedAttack();
        }
    }


    void MeleeAttack()
    {
        // Attack animation
        anim.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    void AltMeleeAttack()
    {
        anim.SetTrigger("SpecialAttack");
    }

    void RangedAttack()
    {

        //    anim.SetTrigger("Throw");
        //    SpawnProjectile();
    }


    public void SpawnProjectile()
    {
        //if (projectile != null)
        //{
        //    // Set correct arrow spawn position
        //    Vector3 facingVector = new Vector3(direction, 1, 1);
        //    Vector3 projectionSpawnPosition = transform.localPosition + Vector3.Scale(projectionSpawnOffset, facingVector);
        //    GameObject bolt = Instantiate(projectile, projectionSpawnPosition, gameObject.transform.localRotation) as GameObject;
        //    // Turn arrow in correct direction
        //    bolt.transform.localScale = facingVector;
        //}
    }



    void FlipCharacterDirection()
    {
        //Turn the character by flipping the direction
        direction *= -1;

        //Record the current scale
        Vector3 scale = transform.localScale;

        //Set the X scale to be the original times the direction
        scale.x = originalXScale * direction;

        //Apply the new scale
        transform.localScale = scale;
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
        //Record the player's position
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

    // Update is called once per frame
    void Update()
    {

    }
}
