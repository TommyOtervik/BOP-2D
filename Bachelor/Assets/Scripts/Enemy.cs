using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    public AIPath aiPath;

    public Transform attackPoint;
    public LayerMask playerLayer;
    [SerializeField]
    private float attackRange = 1f;
    private int attackDamage = 20;


    public int maxHealth = 100;
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        Attack();

        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1.7f, rb.transform.localScale.y, 0);
            // Bare Testing for å se animasjoner
            anim.SetInteger("AnimState", 1);
            anim.SetBool("Grounded", true);
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1.7f, rb.transform.localScale.y, 0);
            // Bare Testing for å se animasjoner
            anim.SetInteger("AnimState", 1);
            anim.SetBool("Grounded", true);
        }


       
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt anim
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Die anim
        anim.SetTrigger("Death");

        // Disable the enemy
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        aiPath.canMove = false;
        
        this.enabled = false;
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger("Attack");
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
