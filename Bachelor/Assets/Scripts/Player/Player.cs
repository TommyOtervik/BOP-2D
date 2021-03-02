using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{


    #region Combat Variables 
    [Header("Attack Properties")]
    
    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField]
    private float attackRange = 1f;
    private const int MELEE_ATTACK_DAMAGE = 20;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Ranged Attack Properties")]
    [SerializeField] GameObject projectile;
    [SerializeField] Vector3 projectionSpawnOffset;

    private const float CAMERA_SHAKE_INTENSITY = 5f;
    private const float CAMERA_SHAKE_DURATION = .1f;
    private const float ATTACK_WAIT_TIME_FOR_SHAKE = .3f;

    #endregion
    

    #region Health System 
    [Header("Health Properties")]
    
    public int maxHealth = 100;
    public int currentHealth;

    #endregion

    PlayerInput input;  //The current inputs for the player
    Animator anim;

    void Start()
    {

        currentHealth = maxHealth;
        

        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

   

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(20);
            
        }

        AttackManager();

        
    }



    void AttackManager()
    {

        if (Time.time >= nextAttackTime)
        {
            if (input.firePressed)
            {
                Attack(MELEE_ATTACK_DAMAGE);
                // If attack rate is 2, add 1 divided by 2 = 0.5 sec 
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("Hurt");
        currentHealth -= damage;

        if (currentHealth <= 0)
            Death();
    }

    void Death()
    {
        
            Debug.Log("Dead?");
            anim.SetTrigger("Death");
 
        // Disable, respawn?
    }

    void Attack(int amount)
    {
        // Attack animation
        anim.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            // FIXME: Scuffed? String check?
            if (enemy.name.Equals("EnemyColliders"))
            {
                StartCoroutine(WaitForAttackDamage(enemy));
            }
        }
    }

    // "Venter" på animasjonen i .3 sekunder. Gjør skade til fiende og rister kameraet.
    IEnumerator WaitForAttackDamage(Collider2D enemy)
    {

        yield return new WaitForSeconds(ATTACK_WAIT_TIME_FOR_SHAKE);

        CinemachineShake.Instance.ShakeCamera(CAMERA_SHAKE_INTENSITY, CAMERA_SHAKE_DURATION);
        enemy.GetComponentInParent<EnemyCultist>().TakeDamage(MELEE_ATTACK_DAMAGE);
    }

  

    // Tegner en sirkel som avgrenser hvor spilleren kan angripe
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
