using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skully : Enemy, IDamageable
{
    // Collision enter etterpå
    private int maxHealth = 30;
    private int currentHealth;
    private Collider2D collider;
    private float speed = 10.0f;

    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointDownLeft;
    [SerializeField] private Transform attackPointDownRight;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform upperLeft;
    [SerializeField] private Transform upperRight;
    [SerializeField] private Transform upperCenter;
    private Transform target;
    private bool attackInProgress = false;

// Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        MovementCheck();
    }



    void Attack1()
    {
        if (target != null)
        {
            return;
        }

        target = upperCenter;
        Debug.Log("Pang Pang masse damage");
    }

    void MovePointToPoint(Transform current, Transform target)
    {
        if (current.position != target.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            target = null;
        }
    }

    void MovementCheck()
    {
        if (target != null)
        {
            MovePointToPoint(transform, target);
        }
    }
    
    void LeftToRight(Transform current, Transform target)
    {
        if (current == upperLeft)
        {
            while (current != target)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            
        }

        else
        {
            while (current != upperLeft)
            {
                transform.position = Vector2.MoveTowards(transform.position, upperLeft.position, speed * Time.deltaTime);
            }
            
            
        }

       
        
    }
    void RightToLeft(Transform current, Transform target)
    {
        
    }
    void Center(Transform current, Transform target)
    {
        
    }
    void SpawnPoint(Transform current, Transform target)
    {
        
    }
    
    
    

    







    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        if (gameObject != null)
        {
            base.MakeLoot();
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
        return collider.gameObject;
    }
}
