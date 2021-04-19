using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAndHealth : Enemy, IDamageable
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int currentHealth;
    [SerializeField] private int collisionDamageAmount = 10;
    
    private float attackRate = 1.5f;
    private const string PLAYER_NAME = "Player";
    
    private Collider2D circleBatCollider;
    public GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        circleBatCollider = GetComponent<CircleCollider2D>();
        currentHealth = maxHealth;
        StartCoroutine(AttackPattern());
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttackPattern()
    {
        // Egentlig while fienden er i livet, evt om vi skal ha en trigger range før første angrep?
        while (true)
        {
            yield return new WaitForSeconds(attackRate);
            HorizontalVerticalAttack();
            yield return new WaitForSeconds(attackRate);
            DiagonalAttack();
        }

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
        return circleBatCollider.gameObject;
    }
}
