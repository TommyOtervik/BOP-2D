using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class Player : MonoBehaviour, IAttacker<int>, IDamageable<int>
{
    #region General Variables
    private PlayerInput input;  //The current inputs for the player
    private Animator anim;
   
    private const string HURT_STR = "Hurt";
    private const string DEATH_STR = "Death";
    private const string ATTACK_STR = "Attack";

    [Header("General Properties")]
    [SerializeField] private Transform spawnPoint;

    #endregion

    #region Health System 
    [Header("Health Properties")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    #endregion

    #region Combat Variables 
    [Header("Attack Properties")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private float attackRange = 1f;
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

    #region Events
    private UnityAction killFloorHitListener;
    private const string KILL_FLOOR_HIT_KEY = "KillFloorHit";
    #endregion


    private void Awake()
    {
        killFloorHitListener = new UnityAction(Death);
    }

    void Start()
    {
        currentHealth = maxHealth;
        
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

   

    private void Update()
    {
       
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
        // if, elif altFire, specialAttack..?
    }

    
    public void TakeDamage(int damage)
    {
        anim.SetTrigger(HURT_STR);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        anim.SetTrigger(DEATH_STR);
        GetComponent<PlayerMovement>().enabled = false;
        
        StartCoroutine(WaitForRespawn());
    }

    // FIXME: Skal i GameManager? -> Må også gjøre spilleren "Usynlig" for fiender når hen er død.
    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(1f);

        // transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Attack(int amount)
    {
        // Attack animation
        anim.SetTrigger(ATTACK_STR);

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            // FIXME: Scuffed? String check? Skulle ha hatt navnet CultistCollider.
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


    private void OnEnable()
    {
        EventManager.StartListening(KILL_FLOOR_HIT_KEY, killFloorHitListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(KILL_FLOOR_HIT_KEY, killFloorHitListener);
    }

    // Getters
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
