using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Tilhører spilleren.
 *  Håndterer bevegelsen / fysikken.
 *   
 */
public class PlayerMovement : MonoBehaviour
{

    public bool drawDebugRaycasts = true;   

    [Header("Movement Properties")]
    [SerializeField] private float speed = 4f;                // Spillerens hastighet
    [SerializeField] private float coyoteDuration = .05f;     // Tidsrommet spilleren kan hoppe før hen faller
    private float maxFallSpeed = -25f;              // Maks. hastighet spilleren faller

    private bool facingRight;   // Retningen spilleren ser i
    public bool FacingRight
    {
        get { return facingRight; }
    }

    [Header("Jump Properties")]
    public float jumpForce = 6.3f;                 // Opprinnelig hoppkraft
    public float jumpHoldForce = 1.9f;             // Inkrementell kraft når hopp holdes
    public float jumpHoldDuration = .1f;           // Hvor lenge hoppeknappen kan holdes
    public float fallMultiplier = 2.5f;            // Fallmultiplikator
    public float lowJumpMultiplier = 2f;           // Lavt-hoppmultiplikator
    private const float BONUS_GRAVITY = 9.8f;      // Bonus gravitasjon, brukes for å gjøre hoppet mer "urealistisk"
    private float jumpTime;						   // Variabel for å holde hoppvarighet


    [Header("Environment Check Properties")]
    // Facing right
    [SerializeField] private float footOffsetLeftFacingRight = 0.95f;
    [SerializeField] private float footOffsetRightFacingRight = 0.75f;
    // Facing left
    [SerializeField] private float footOffsetLeftFacingLeft = 0.75f;
    [SerializeField] private float footOffsetRightFacingLeft = 0.95f;


    [SerializeField] private float eyeHeight = 1.11f;           // Høyde på veggsjekker
    [SerializeField] private float headClearance = .25f;        // Plass som trengs over spillerens hode
    [SerializeField] private float groundDistance = .35f;       // Avstanden som regnes som bakken
    [SerializeField] private LayerMask groundLayer;             // Layer for bakken (plattformene)
    

    [Header("Status Flags")]
    [SerializeField] private bool isOnGround;                 // Er spilleren på bakken?
    [SerializeField] private bool isJumping;                  // Hopper spilleren?
    [SerializeField] private bool isHeadBlocked;

    PlayerInput input;                      // Gjeldende inputs for spilleren
    BoxCollider2D bodyCollider;             // Collider-komponenten
    Rigidbody2D rigidBody;                  // Rigidbody-komponenten
    Animator anim;                          // Animator-komponenten
    PlayerMovement movement;                // PlayerMovement-komponenten


    private float delayToIdle = 0.0f;
    private const float DELAY_TO_IDLE_ANIM = 0.05f;
    private float coyoteTime;                       // Variabel for å holde coyote-time


    private float originalXScale;                   // Original scale på X-aksen
    private int direction = 1;                      // Retning


    void Start()
    {
        // Få en referanse til de nødvendige komponentene
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        movement = GetComponent<PlayerMovement>();

        // Hvis noen av de nødvendige komponentene ikke eksisterer ...
        if (movement == null || rigidBody == null || input == null || anim == null)
        {
            // ... logg en feil og fjern deretter denne komponenten
            Debug.LogError("A needed component is missing from the player");
            Destroy(this);
        }

        facingRight = true;
      
        originalXScale = transform.localScale.x;
    }
    

    void FixedUpdate()
    {
        // Sjekk området rudt for å bestemme status
        PhysicsCheck();

        // Behandle bakke- og luftbevegelser
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        // Start med å anta at spilleren ikke er på bakken og at hodet ikke er blokkert
        isOnGround = false;
        isHeadBlocked = false;

        // Raycast for venstre og høyre fot
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


        // Hvis en av strålene traff bakken, er spilleren på bakken
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

        // Raycast for å sjekke over spillerens hode
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

        // Hvis den strålen treffer, er spillerhodet blokkert
        if (headCheck)
            isHeadBlocked = true;
    }

    void GroundMovement()
    {
        // Beregn ønsket hastighet basert på inputs
        float inputX = Input.GetAxis("Horizontal");
        float xVelocity = speed * inputX;
        // Hvis hastighet og retning ikke stemmer overens, snu.
        if (xVelocity * direction < 0f)
            FlipCharacterDirection();

        // Bruk ønsket hastighet
        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);


        // Hvis spilleren er på bakken, utvider du coyotetidsvinduet
        if (isOnGround)
        {
            coyoteTime = Time.time + coyoteDuration;
        }

        // Løp
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            delayToIdle = DELAY_TO_IDLE_ANIM;
            anim.SetInteger("AnimState", 1);
        }
        // Idle (Stå i ro)
        else
        {
            // Hindrer flimrende overganger i animasjon
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                anim.SetInteger("AnimState", 0);
        }
    }

    void MidAirMovement()
    {

        // Hvis man trykker hopp og man er på bakken og hodet ikke er blokkert
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {

            isOnGround = false;
            isJumping = true;
            // Setter animasjonene
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", isOnGround);
            // Holder styr på hopp tiden
            jumpTime = Time.time + jumpHoldDuration;
            // Hopp hastighet
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 4);

        }
        // Hvis man allerede hopper
        else if (isJumping)
        {
            // Sjekk om man holder inne knappen
            if (input.jumpHeld)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce * jumpHoldForce);

            if (jumpTime <= Time.time)
                isJumping = false;

        }

        // Legger til ekstra falsk gravitasjon til spilleren
        // Dette gjør hoppet mindre realistisk, på en god måte
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

        // Hvis spilleren faller for raskt, reduserer du Y-hastigheten
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);

        
    }


    void FlipCharacterDirection()
    {
        // Snu karakteren ved å snu retningen
        direction *= -1;

        // Registrer gjeldende scale
        Vector3 scale = transform.localScale;

        // Sett X-scale til å være originalen ganger retningen
        scale.x = originalXScale * direction;

        // Bruk den nye scale
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


}
