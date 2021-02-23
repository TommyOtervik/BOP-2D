using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    [Header("Movement Properties")]
    public float speed = 4f;                //Player speed
    public float coyoteDuration = .05f;     //How long the player can jump after falling
    float maxFallSpeed = -25f;       //Max speed player can fall

    // facing direction
    private bool facingRight;

    [Header("Jump Properties")]
    public float jumpForce = 6.3f;          //Initial force of jump
    public float jumpHoldForce = 1.9f;      //Incremental force when jump is held
    public float jumpHoldDuration = .1f;    //How long the jump key can be held
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    private const float BONUS_GRAVITY = 9.8f;
    private float jumpTime;							//Variable to hold jump duration



    [Header("Attack Properties")]
    // Testing for angrep
    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField]
    private float attackRange = 1f;
    private int attackDamage = 20;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;
    // End Test Angrep


    [Header("Environment Check Properties")]
    // Facing right
    public float footOffsetLeftFacingRight = 0.95f;
    public float footOffsetRightFacingRight = 0.75f;
    // Facing left
    public float footOffsetLeftFacingLeft = 0.75f;
    public float footOffsetRightFacingLeft = 0.95f;
    
    
    //public float footOffset = .32f;          //X Offset of feet raycast
    public float eyeHeight = 1.11f;          //Height of wall checks
    public float headClearance = .25f;       //Space needed above the player's head
    public float groundDistance = .35f;      //Distance player is considered to be on the ground
    public LayerMask groundLayer;           //Layer of the ground
    

    [Header("Status Flags")]
    public bool isOnGround;                 //Is the player on the ground?
    public bool isJumping;                  //Is player jumping?
    public bool isHeadBlocked;

    PlayerInput input;                      //The current inputs for the player
    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component
    Animator anim;
    PlayerMovement movement;


    [SerializeField] GameObject projectile;
    [SerializeField] Vector3 projectionSpawnOffset;


    float delayToIdle = 0.0f;
    float coyoteTime;                       //Variable to hold coyote duration
    float playerHeight;                     //Height of the player

    float originalXScale;                   //Original scale on X axis
    int direction = 1;                      //Direction player is facing

    Vector2 colliderStandSize;              //Size of the standing collider
    Vector2 colliderStandOffset;			//Offset of the standing collider


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

        facingRight = true;
        
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
        RaycastHit2D leftCheck;
        RaycastHit2D rightCheck;
        
        if (facingRight)
        {
            leftCheck = Raycast(new Vector2(-footOffsetLeftFacingRight, .2f), Vector2.down, groundDistance);
            rightCheck = Raycast(new Vector2(footOffsetRightFacingRight, .2f), Vector2.down, groundDistance);
        }
        else
        {
            leftCheck = Raycast(new Vector2(-footOffsetLeftFacingLeft, .2f), Vector2.down, groundDistance);
            rightCheck = Raycast(new Vector2(footOffsetRightFacingLeft, .2f), Vector2.down, groundDistance);
        }



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

        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {

            isOnGround = false;
            isJumping = true;

            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", isOnGround);

            jumpTime = Time.time + jumpHoldDuration;

            // rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 4);

        }
        else if (isJumping)
        {

            if (input.jumpHeld)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce * jumpHoldForce);
                // rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

                // Uniform acceleration, "Vanlig"
                // rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight)); 

            if (jumpTime <= Time.time)
                isJumping = false;
        }

        // Add extra fake gravity to the player
        Vector2 vel = rigidBody.velocity;
        vel.x = rigidBody.velocity.x;
        vel.y -= BONUS_GRAVITY * Time.deltaTime;
        rigidBody.velocity = vel;



        anim.SetFloat("AirSpeedY", rigidBody.velocity.y);

        if (rigidBody.velocity.y < 0)
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        }
        else if (rigidBody.velocity.y > 0 && !input.jumpHeld)
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


  
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
            Debug.Log("Hit");
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

        if (facingRight)
        {
            facingRight = false;
        }
        else
        {
            facingRight = true;
        }
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
