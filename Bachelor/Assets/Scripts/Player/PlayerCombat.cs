using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [Header("Attack Properties")]
    // Testing for angrep
    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField]
    private float attackRange = 1f;
    private int attackDamage = 20;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;
    // End Test Angrep


    PlayerInput input;                      //The current inputs for the player
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AttackManager();
    }

    void AttackManager()
    {

        if (Time.time >= nextAttackTime)
        {
            if (input.firePressed)
            {
                MeleeAttack();
                // If attack rate is 2, add 1 divided by 2 = 0.5 sec 
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }

        if (input.altFirePressed)
        {
            AltMeleeAttack();
        }
        else if (input.rangedAttack)
        {
            RangedAttack();
        }
    }


    void MeleeAttack()
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

        yield return new WaitForSeconds(.3f);
        CinemachineShake.Instance.ShakeCamera(5f, .1f);
        enemy.GetComponentInParent<Enemy>().TakeDamage(attackDamage);
    }

    // Tegner en sirkel som avgrenser hvor spilleren kan angripe
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    void AltMeleeAttack()
    {
        anim.SetTrigger("SpecialAttack");
    }

    void RangedAttack()
    {

        //    anim.SetTrigger("Throw");
        //    SpawnProjectile();
    }


    public void SpawnProjectile()
    {
        //if (projectile != null)
        //{
        //    // Set correct arrow spawn position
        //    Vector3 facingVector = new Vector3(direction, 1, 1);
        //    Vector3 projectionSpawnPosition = transform.localPosition + Vector3.Scale(projectionSpawnOffset, facingVector);
        //    GameObject bolt = Instantiate(projectile, projectionSpawnPosition, gameObject.transform.localRotation) as GameObject;
        //    // Turn arrow in correct direction
        //    bolt.transform.localScale = facingVector;
        //}
    }
}
