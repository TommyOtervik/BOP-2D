using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbominationMiniBoss : MonoBehaviour, IDamageable
{

    #region External Private Variables
    [Header("Abomination Info")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;
    [SerializeField] private float cooldown;
    [SerializeField] private bool isEnraged;
    [SerializeField] private Transform target;
    [SerializeField] private AbominationHealthBar healthBar;

    [SerializeField] private GameObject flame;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform enragePoint;

    [SerializeField] private Transform enrageHitStartPoint;
    [SerializeField] private Transform enrageHitEndPoint;

    [SerializeField] private GameObject hotZone;

    // BoxCollider for våpen til fiende
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private int currentHealth;
    #endregion


    #region Internal Private Variables
    private Animator anim;
    private float distance; // Store distance b/w enemy and player
    private bool attackMode;
    private bool cooling; // Check if enemy is cooling after attack
    private bool insideHotZone; // Check if player inside
    private float enrageTimer; // Enrage Timer
    private int minRandomHurt = 1;
    private int maxRandomHurt = 10;
    private float startX, endX, flameDistance; // Enraged attack distance variables

    private float timeForNextAttack;
    private Collider2D abomCollider;


    #endregion
    private UnityAction playerDeadListener;

    // Start is called before the first frame update
    void Awake()
    {
        enrageTimer = 5;

        startX = enrageHitStartPoint.position.x;
        endX = enrageHitEndPoint.position.x;
        flameDistance = (startX - endX);

        playerDeadListener = new UnityAction(ResetAbomination);

        anim = GetComponent<Animator>();
        abomCollider = GetComponent<BoxCollider2D>();

        currentHealth = maxHealth;

        healthBar.SetSize((float)currentHealth / (float)maxHealth);

        anim.SetBool("canWalk", false);
    }

    // Update is called once per frame
    void Update()
    {
        enrageTimer -= Time.deltaTime;
        if (enrageTimer <= 0)
            isEnraged = true;

        if (insideHotZone && !isEnraged)
            EnemyLogic();
        else if (insideHotZone && isEnraged)
            Enrage();
        else
            ResetAbomination();
    }

    private void EnemyLogic()
    {
        speed = 2f;

        distance = Vector2.Distance(transform.position, target.position);
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Abomination_Attack"))
        {
            Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        if (distance > attackRange)
            StopAttack();
        else if (distance <= attackRange && !cooling)
            Attack();

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }


        Flip(target);
    }
    private void Enrage()
    {
        StopAttack();

        speed = 5f;

        Vector2 targetPos = new Vector2(enragePoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Flip(enragePoint);

        if (transform.position.x == enragePoint.position.x)
        {
            anim.SetBool("canWalk", false);
            Flip(target);
            anim.SetBool("isEnraged", true);

            StartCoroutine(WaitForEnrage());
        }
    }


    private IEnumerator WaitForEnrage()
    {
        yield return new WaitForSeconds(4f);
        isEnraged = false;
        enrageTimer = 20;
        anim.SetBool("canWalk", true);
        anim.SetBool("isEnraged", false);
    }


    private void Cooldown()
    {
        timeForNextAttack -= Time.deltaTime;
        if (timeForNextAttack <= 0 && cooling)
        {
            cooling = false;
            timeForNextAttack = cooldown;
        }
    }

    private void Attack()
    {
        timeForNextAttack = cooldown;
        attackMode = true;

        anim.SetBool("canWalk", !attackMode);
        anim.SetBool("Attack", attackMode);
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
    }

    public void SpawnFlame()
    {
        StartCoroutine(FlameDelay(flame));
    }

    IEnumerator FlameDelay(GameObject f)
    {
        
        for (float i = startX; i >= flameDistance; i -= 2f)
        {
            Vector3 pos = new Vector3((startX - i), 0f);

            GameObject flame = Instantiate(f, enrageHitStartPoint.position - pos, Quaternion.identity, null);


            if(flame.transform.position.x <= endX)
            {
                Destroy(flame);
                break;
            }

            yield return new WaitForSeconds(.05f);

            Destroy(flame, .5f);
         
        }
    }

    private void ResetAbomination()
    {
        StopAttack();

        speed = 5f;
        currentHealth = maxHealth;
        healthBar.SetSize((float)currentHealth / (float)maxHealth);

        Vector2 targetPos = new Vector2(startPoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position.x == startPoint.position.x)
        {
            anim.SetBool("canWalk", false);
        }

        Flip(startPoint);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(abomCollider, collision.collider, true);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isEnraged)
        {
            currentHealth -= damage;
            healthBar.SetSize((float)currentHealth / (float)maxHealth);
        }

        int hurtRand = UnityEngine.Random.Range(minRandomHurt, maxRandomHurt + 1);
        if (hurtRand == 1)
        {
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        // Die anim
        anim.SetTrigger("Death");
        attackMode = false;
        anim.SetBool("Attack", attackMode);

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        // Disable the enemy
        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in enemyColliders)
            c.enabled = false;

        this.enabled = false;

        EventManager.TriggerEvent(EnumEvents.ABOMINATION_DEAD);
    }

    // Brukes i animator for å vente med å angripe etter et slag.
    public void TriggerCooling()
    {
        cooling = true;
    }

    public void Flip(Transform tar)
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > tar.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;

        transform.eulerAngles = rotation;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.PLAYER_DEAD, playerDeadListener);
        DamageBroker.AddToEnemyList(this);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.PLAYER_DEAD, playerDeadListener);
        DamageBroker.RemoveEnemyFromList(this);
    }

    public void SetInsideHotZone(bool b)
    {
        this.insideHotZone = b;
    }
 
    public GameObject GetEnemyGameObject()
    {
        return abomCollider.gameObject;
    }
}
