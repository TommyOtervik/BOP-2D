using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowCultistScript : MonoBehaviour, IDamageable
{

    #region External Private Variables (For editor)
    [Header("EnemyCultist Info")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float attackDistance; // Min. distance for attack
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer; // Timer for cooldown between attacks
    [SerializeField] private bool inRange; // Check if player is in range
    [SerializeField] private Transform target;


    // Søke / trigger område for AI
    [SerializeField] private GameObject hotZone;
    [SerializeField] private GameObject triggerArea;

    // BoxCollider for våpen til fiende
    [SerializeField] private BoxCollider2D hitBox;

    [SerializeField] private int currentHealth;
    #endregion

    #region Internal Private Variables
    private Animator anim;
    private float distance; // Store distance b/w enemy and player
    private bool attackMode;

    private bool cooling; // Check if enemy is cooling after attack
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;
    private float intTimer;

    private Collider2D cultistCollider;
    #endregion

    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetEnemyGameObject()
    {
        return cultistCollider.gameObject;
    }

    public void TakeDamage(int damageTaken)
    {
        throw new System.NotImplementedException();
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        anim.SetInteger("AnimSate", 0);

        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
        {
            if (c.name.Equals("EnemyColliders"))
                cultistCollider = c;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // FIXME: == 12, Equals?
        if (collision.gameObject.layer == 12 && collision.collider.name.Equals("EnemyColliders"))
        {
            Physics2D.IgnoreCollision(cultistCollider, collision.collider, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

