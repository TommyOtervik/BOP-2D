using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*
 * Dette skriptet tilhører Abomination (Mini-boss). 
 * Det håndterer Helse, States, Angrep og Bevegelse.
 * 
 * @ AOP - 225280
 */
public class AbominationMiniBoss : Enemy, IDamageable
{

    #region External Private Variables
    [Header("Abomination Info")]
    [SerializeField] private int maxHealth;     // Maksimum helse
    [SerializeField] private float attackRange; // Angreps rekkevidde
    [SerializeField] private float speed;       // Hastighet
    [SerializeField] private float cooldown;    // Cooldown mellom attacks
    [SerializeField] private bool isEnraged;    // Enraged 
    [SerializeField] private Transform target;  // Mål / Spilleren

    [SerializeField] private AbominationHealthBar healthBar;  // Helsebar (Skulle blitt håndtert via UIManager)

    [SerializeField] private GameObject flame;  // Flamme objektet som instantieres

    [SerializeField] private Transform startPoint;  // Start posisjon til Abom.
    [SerializeField] private Transform enragePoint; // Enrage posisjon til Abom.

    [SerializeField] private Transform enrageHitStartPoint;  // Punktet der flammen starter
    [SerializeField] private Transform enrageHitEndPoint;    // Punktet der flammen slutter

    [SerializeField] private GameObject hotZone; // HotZone (Følger etter spilleren innenfor dette området)

    // BoxCollider for våpen til fiende
    [SerializeField] private BoxCollider2D hitBox; // HitBox (Våpenet til Abom.)
    [SerializeField] private int currentHealth;    // Nåværende helsepoeng
    #endregion


    #region Internal Private Variables
    private Animator anim;      // Animator 
    private float distance;     // Distansen mellom Abom. og spilleren 
    private bool attackMode;    // Angrepsmodus
    private bool cooling;       // Sjekker om Abom. angrep "kjøler ned" etter angrep
    private bool insideHotZone; // Sjekker om spilleren er på insiden av sonen
    private float enrageTimer;  // Enrage TImer
    private bool isDead;        // Sjekker om Abom. er død

    private const int MIN_RANDOM_HURT = 1;  // Brukes til animajson, gir 10% sjanse til å sette Abom. i "Hurt" anim.
    private const int MAX_RANDOM_HURT = 10;
    private const float ENRAGE_WAIT_TIMER = 4f; // Brukes til Enrage
    private float startX, endX, flameDistance; // Hjelpevariabler til "Enraged Attack"

    private float timeForNextAttack; // Holder styr på angrep (tid)
    
    private Collider2D abomCollider; 

    #endregion
    private UnityAction playerDeadListener; // Lytter om spilleren er død
    

    // Start is called before the first frame update
    private void Start()
    {
       
        // Sjekker om Abom. allerede er drept.
        // Hvis den er det, sett helse til 0.
        if (!GameManager.IsAbomDead())
            currentHealth = maxHealth;
        else
            currentHealth = 0;

        if (currentHealth <= 0)
            Death();
        
    }

    void Awake()
    {
        this.enrageTimer = 1000f;

        // Instantiate Enrage variablene
        startX = enrageHitStartPoint.position.x;
        endX = enrageHitEndPoint.position.x;
        flameDistance = (startX - endX);

        // Lytter, dør spilleren -> Gå tilbake til start pos.
        playerDeadListener = new UnityAction(ResetAbomination);
       

        anim = GetComponent<Animator>();
        abomCollider = GetComponent<BoxCollider2D>();

        // UI-håndtering, skulle vært via UIManager.
        healthBar.SetSize((float)currentHealth / (float)maxHealth);

        anim.SetBool("canWalk", false);
    }

   
    void Update()
    {
        // Enrage timer
        enrageTimer -= Time.deltaTime;
        if (enrageTimer <= 0)
            isEnraged = true;


        if (insideHotZone && !isEnraged)
            AbominationLogic();
        else if (insideHotZone && isEnraged)
            Enrage();
        else
            ResetAbomination();

    }

    private void AbominationLogic()
    {
        // Setter hastigheten
        speed = 2f;

        // Henter spillerens posisjon, kalkulerer avstand.
        distance = Vector2.Distance(transform.position, target.position);
        // Setter "gå" animasjonen
        anim.SetBool("canWalk", true);

        // Sjekker om Abom. er i angrepsanimasjonen
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Abomination_Attack"))
        {
            Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

      
        if (distance > attackRange)                     // Hvis spilleren kommer seg langt nok unna -> slutt å angripe
            StopAttack();
        else if (distance <= attackRange && !cooling)   // Hvis spilleren er innenfor rekkevidde -> angrip
            Attack();

        // "Kjøl ned", kan ikke angripe (blir satt i animasjonen)
        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }

        // Snu Abom. mot target
        Flip(target);
    }
    private void Enrage()
    {
        // Slutt å angripe
        StopAttack();

        // Sett hastighet
        speed = 5f;

        // Gå mot "Enrage-point" (Ved døren)
        Vector2 targetPos = new Vector2(enragePoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Snu Abom. mot dette punktet
        Flip(enragePoint);

        // Hvis Abom. når punktet
        if (transform.position.x == enragePoint.position.x)
        {
            anim.SetBool("canWalk", false); 
            Flip(target);                    // Snu mot spilleren
            anim.SetBool("isEnraged", true); // Start animasjon

            StartCoroutine(WaitForEnrage(ENRAGE_WAIT_TIMER)); // Vent 4 sekunder
        }
    }


    private IEnumerator WaitForEnrage(float sec)
    {
        yield return new WaitForSeconds(sec);
        isEnraged = false;
        enrageTimer = 30;
        anim.SetBool("canWalk", true);
        anim.SetBool("isEnraged", false);
    }


    // Cooldown metode
    private void Cooldown()
    {
        timeForNextAttack -= Time.deltaTime;
        if (timeForNextAttack <= 0 && cooling)
        {
            cooling = false;
            timeForNextAttack = cooldown;
        }
    }

    // Angreps metode
    private void Attack()
    {
        timeForNextAttack = cooldown;
        attackMode = true;

        anim.SetBool("canWalk", !attackMode);
        anim.SetBool("Attack", attackMode);
    }

    // Stopp-angrep metode
    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", attackMode);
    }

    // Setter "fyr" på bakken, blir kalt i animasjon.
    public void SpawnFlame()
    {
        StartCoroutine(FlameDelay(flame));
    }

    IEnumerator FlameDelay(GameObject f)
    {

        // For-løkke for å instantiate flamme objekter, baserer seg på start og slutt punkt.
        // Variabler blir satt i Start metoden.
        /*
         *       startX = enrageHitStartPoint.position.x;
         *       endX = enrageHitEndPoint.position.x;
         *       flameDistance = (startX - endX);
         */
        for (float i = startX; i >= flameDistance; i -= 2f)
        {
            Vector3 pos = new Vector3((startX - i), 0f);
            // Nytt objekt
            GameObject flame = Instantiate(f, enrageHitStartPoint.position - pos, Quaternion.identity, null);
            // Hvis flammen når slutt punktet, slett objektet og hopp ut.
            if(flame.transform.position.x <= endX)
            {
                Destroy(flame);
                break;
            }
            yield return new WaitForSeconds(.05f);
            // Venter .05 sekunder før man sletter objektet
            Destroy(flame, .5f);   
        }
    }

    // "Tilbakestiller" Abom. blir kalt hvis den dreper spilleren.
    private void ResetAbomination()
    {
        StopAttack(); // Stopper angrep

        speed = 5f;

        // Setter helsen tilbake
        currentHealth = maxHealth; 
        healthBar.SetSize((float)currentHealth / (float)maxHealth); 

        // Velger nytt punkt å gå til, i dette tilfellet startposisjonen
        Vector2 targetPos = new Vector2(startPoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Når den når pos. -> stå stille.
        if (transform.position.x == startPoint.position.x)
            anim.SetBool("canWalk", false);
        

        Flip(startPoint);
    }

    // Ignorer kollisjon med spilleren
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Physics2D.IgnoreCollision(abomCollider, collision.collider, true);
        
    }

    // Tar skade metode, så lengde den ikke er i Enrage mode
    public void TakeDamage(int damage)
    {
        if (!isEnraged)
        {
            currentHealth -= damage;
            healthBar.SetSize((float)currentHealth / (float)maxHealth);
        }

        int hurtRand = UnityEngine.Random.Range(MIN_RANDOM_HURT, MAX_RANDOM_HURT + 1);
        if (hurtRand == 1)
            anim.SetTrigger("Hurt");
        

        if (currentHealth <= 0)
        {
            MakeLoot();
            Death();
        }
   
    }

    // Instantiate nøkkel
    protected override void MakeLoot()
    {
        if (thisLoot != null)
        {
            Pickup current = thisLoot.LootPickup();
            if (current != null)
            {
                Instantiate(current.gameObject, new Vector2(transform.position.x + 0.5f, transform.position.y + 3), Quaternion.identity);
            }
        }
    }

    // Håndterer død. Deaktiverer komponenter knyttet til Abom.
    // Og sender beskjed til Events og GameManager
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

        isDead = true;

        EventManager.TriggerEvent(EnumEvents.ABOMINATION_DEAD);
        GameManager.PlayerKilledAbom(isDead);
    }

    // Brukes i animator for å vente med å angripe etter et slag.
    public void TriggerCooling()
    {
        cooling = true;
    }

    // Snur Abom. basert på en posisjon
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

    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }
    public void SetInsideHotZone(bool b)
    {
        this.insideHotZone = b;
    }

    public void SetEnrageTimer(float timer)
    {
        this.enrageTimer = timer;
    }

    public bool GetIsDead()
    {
        return this.isDead;
    }
 
    public GameObject GetEnemyGameObject()
    {
        return abomCollider.gameObject;
    }
}
