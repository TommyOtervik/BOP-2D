using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    private float speed = 4f;                //Player speed
    // public float crouchSpeedDivisor = 3f;   //Speed reduction when crouching
    private float coyoteDuration = .05f;     //How long the player can jump after falling
    private float maxFallSpeed = -25f;       //Max speed player can fall




    private float jumpForce = 2f;          //Initial force of jump
    // public float crouchJumpBoost = 2.5f;    //Jump boost when crouching
    // public float hangingJumpForce = 15f;    //Force of wall hanging jump
    private float jumpHoldForce = 1f;      //Incremental force when jump is held
    private float jumpHoldDuration = .1f;    //How long the jump key can be held

    [Header("Environment Check Properties")]
    public float footOffset = .32f;          //X Offset of feet raycast
    public float eyeHeight = 1.11f;          //Height of wall checks
    public float reachOffset = .47f;         //X offset for wall grabbing
    public float headClearance = .25f;       //Space needed above the player's head
    public float groundDistance = .35f;      //Distance player is considered to be on the ground
    public float grabDistance = .4f;        //The reach distance for wall grabs
    public LayerMask groundLayer;           //Layer of the ground

    private bool isOnGround = false;                 //Is the player on the ground?
    private bool isJumping;                  //Is player jumping?
    // public bool isHanging;                  //Is player hanging?
    // public bool isCrouching;                //Is player crouching?
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
        MeleeAttack();
        RangedAttack();
    }

    void PhysicsCheck()
    {
        //Start by assuming the player isn't on the ground and the head isn't blocked
        isOnGround = false;
        isHeadBlocked = false;

        //Cast rays for the left and right foot
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, .1f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, .1f), Vector2.down, groundDistance);

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
            anim.SetBool("Grounded", isOnGround);
        }
        else
        {
            isOnGround = false;
            anim.SetBool("Grounded", isOnGround);
        }
            

        //Cast the ray to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

        //If that ray hits, the player's head is blocked
        if (headCheck)
            isHeadBlocked = true;


        //Determine the direction of the wall grab attempt
        // Vector2 grabDir = new Vector2(direction, 0f);

        //Cast three rays to look for a wall grab
       // RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance);
       // RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
       // RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance);
    }

    void GroundMovement()
    {
        //Calculate the desired velocity based on inputs
        float inputX = Input.GetAxis("Horizontal");
        float xVelocity = speed * input.horizontal;
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
        if(Mathf.Abs(inputX) > Mathf.Epsilon)
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

    // FIXME: Ikke helt 100%, SetTrigger("Jump") registreres ikke i Animator.
    void MidAirMovement()
    {
        anim.SetFloat("AirSpeedY", rigidBody.velocity.y);

        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time))
        {
            anim.SetTrigger("Jump");
            isOnGround = false;
            isJumping = true;

            anim.SetBool("Grounded", isOnGround);
            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;

            //...add the jump force to the rigidbody...
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            
        }
        else if (isJumping)
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            //...and if jump time is past, set isJumping to false
            if (jumpTime <= Time.time)
                isJumping = false;
        }


        //If player is falling to fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);

    }

    void MeleeAttack()
    {
        if(input.firePressed)
        {
            anim.SetTrigger("Attack");
        }
        else if(input.altFirePressed)
        {
            anim.SetTrigger("SpecialAttack");
        }
    }

    void RangedAttack()
    {
        if (input.rangedAttack)
        {
            anim.SetTrigger("Throw");
            SpawnProjectile();
        }
    }


    public void SpawnProjectile()
    {
        if (projectile != null)
        {
            // Set correct arrow spawn position
            Vector3 facingVector = new Vector3(direction, 1, 1);
            Vector3 projectionSpawnPosition = transform.localPosition + Vector3.Scale(projectionSpawnOffset, facingVector);
            GameObject bolt = Instantiate(projectile, projectionSpawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            bolt.transform.localScale = facingVector;
        }
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
