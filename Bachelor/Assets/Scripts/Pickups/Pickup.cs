
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    protected const string PLAYER_NAME = "Player";
    protected Vector2 startPosition;
    protected Vector2 upperBound;
    protected Vector2 lowerBound;
    protected float speed = 1.0f;
    protected float fallSpeed = 2.0f;
    protected bool movingUp = true;
    protected float upperBoundYOffset = 0.6f;
    

    //Raycast stuff
    public LayerMask groundLayer;
    public bool drawDebugRaycasts = true;
    public bool hitGround;
    public BoxCollider2D collider;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        upperBound = new Vector2(startPosition.x, startPosition.y + upperBoundYOffset);
        lowerBound = startPosition;
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsCheck();
        Move();
    }
    

    void Move()
    {
        if (!hitGround)
        {
            //transform.position = Vector2.MoveTowards(transform.position, Vector2.right, speed * Time.deltaTime);
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }

        else
        {
            if (movingUp)
            {
                if (transform.position.y >= upperBound.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, lowerBound, speed * Time.deltaTime);
                    movingUp = false;
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, upperBound, speed * Time.deltaTime);
                }

            
            }
        
            else if (!movingUp)
            {
                if (transform.position.y <= lowerBound.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, upperBound, speed * Time.deltaTime);
                    movingUp = true;
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, lowerBound, speed * Time.deltaTime);
                }

            
            }
        }

        

    }

    void PhysicsCheck()
    {
        if (hitGround)
        {
            return;
        }

        RaycastHit2D groundCheck = Raycast(new Vector2(0f, collider.size.y), Vector2.down, 0.3f);
        if (groundCheck)
        {
            // Overstyrer upperbound og lowerbound i det jeg treffer bakken for å unngå at pickup går mot originalposisjon
            lowerBound = transform.position;
            upperBound = new Vector2(lowerBound.x, lowerBound.y + upperBoundYOffset);
            hitGround = true;
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
