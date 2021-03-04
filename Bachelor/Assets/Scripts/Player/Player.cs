using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class Player : MonoBehaviour, IAttacker<int>, IDamageable<int>
{
    #region General Variables
    private PlayerInput input;  // Gjendende input for spilleren
    private Animator anim;    // Animator, setter animasjoner basert på funksjoner/variabler

    // Konstanter for animator
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

    private const string PLAYER_DEAD_KEY = "PlayerDead";
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

   
    // Håndtere angrep (hvordan knapp som blir trykket på)
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

    // Spilleren tar skade
    public void TakeDamage(int damage)
    {
        anim.SetTrigger(HURT_STR);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Death();
    }

    // Spilleren dør
    public void Death()
    {
        anim.SetTrigger(DEATH_STR);

        // Gjør spilleren "unsynlig" for fiender når hen dør.
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<BoxCollider2D>().enabled = false;

        // Fiender lytter til denne
        EventManager.TriggerEvent(PLAYER_DEAD_KEY);

        StartCoroutine(WaitForRespawn());
    }

    // FIXME: Skal i GameManager? (LoadScene..)
    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(3f);

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

    // Event lytter når objektet blir aktiv
    private void OnEnable()
    {
        EventManager.StartListening(KILL_FLOOR_HIT_KEY, killFloorHitListener);
    }

    // Slå av lytter når objektet blir inaktivt (Memory leaks)
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
