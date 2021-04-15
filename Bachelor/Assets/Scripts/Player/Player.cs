using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class Player : MonoBehaviour, IAttacker<int>, IDamageable
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

    // TEST FOR SAVE (SKAL EGENTLIG I GAMEMANAGER?)
    private UnityAction tutorialToCastleListener;
    private UnityAction loadPlayerListener;

    public static event Action<int> UpdateHealth;
    public static event Action<int> SetMaxHealth;
    #endregion


    private void Awake()
    {
        currentHealth = maxHealth;

        killFloorHitListener = new UnityAction(Death);
        tutorialToCastleListener = new UnityAction(SavePlayer);
        loadPlayerListener = new UnityAction(LoadPlayer);
    }

    void Start()
    {
        
        SetMaxHealth?.Invoke(maxHealth);

        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }



    private void Update()
    {  
        AttackManager();

        UpdateHealth?.Invoke(currentHealth);  
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
        // Quick fix..
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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
        EventManager.TriggerEvent(EnumEvents.PLAYER_DEAD);

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
            //StartCoroutine(WaitForAttackDamage(enemy)); 
            DamageBroker.EnemyTakesDamage(MELEE_ATTACK_DAMAGE, enemy.gameObject);
        }
    }

    /*
    // "Venter" på animasjonen i .3 sekunder. Gjør skade til fiende og rister kameraet.
    IEnumerator WaitForAttackDamage(Collider2D enemy)
    {
        yield return new WaitForSeconds(ATTACK_WAIT_TIME_FOR_SHAKE);

        CinemachineShake.Instance.ShakeCamera(CAMERA_SHAKE_INTENSITY, CAMERA_SHAKE_DURATION);
        DamageBroker.EnemyTakesDamage(MELEE_ATTACK_DAMAGE, enemy.gameObject);
      
    }
    */
    

  

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
       EventManager.StartListening(EnumEvents.KILL_FLOOR_HIT, killFloorHitListener);
       EventManager.StartListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
       EventManager.StartListening(EnumEvents.LOAD_PLAYER, loadPlayerListener);

       DamageBroker.TakeDamageEvent += TakeDamage;
    }

    // Slå av lytter når objektet blir inaktivt (Memory leaks)
    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.KILL_FLOOR_HIT, killFloorHitListener);
        EventManager.StopListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
        EventManager.StopListening(EnumEvents.LOAD_PLAYER, loadPlayerListener);

        DamageBroker.TakeDamageEvent -= TakeDamage;
    }


    // Brukes i PlayerData for save 
    public int GetCurrentHealth()
    {
        return currentHealth;
    }



    // SKAL I GAME MANAGER
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);

        Debug.Log("Saved Player. HP: " + this.currentHealth);
        Debug.Log("Pos: " + this.transform.position + "- Scene:" + SceneManager.GetActiveScene().name);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        this.currentHealth = data.GetCurrentHealth();

        Debug.Log("Load Player. HP: " + this.currentHealth);
        // BARE TEST FOR CASTLE NIVÅ
        //Vector3 pos;
        //pos.x = -6.7f;
        //pos.y = -1f;
        //pos.z = 0f;

        //transform.position = pos;
    }

    public GameObject GetEnemyGameObject()
    {
        throw new NotImplementedException();
    }
}
