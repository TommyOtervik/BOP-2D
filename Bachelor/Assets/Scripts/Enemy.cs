using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

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
