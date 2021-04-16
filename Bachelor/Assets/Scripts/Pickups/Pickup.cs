
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    protected const string PLAYER_NAME = "Player";
    protected Vector2 startPosition;
    protected Vector2 upperBound;
    protected Vector2 lowerBound;
    protected float speed = 1.0f;
    protected bool movingUp = true;
    protected float upperBoundYOffset = 0.6f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        upperBound = new Vector2(startPosition.x, startPosition.y + upperBoundYOffset);
        lowerBound = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    

    void Move()
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
