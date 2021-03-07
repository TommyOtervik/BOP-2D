using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLaunchEnemy : MonoBehaviour
{
    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    public LayerMask groundLayer;

    private bool canSeePlayer;
    private bool horizontalAttackInProgress;
    private bool grenadeAttackInProgress;
    BoxCollider2D bodyCollider;
    
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;

    [SerializeField] Transform horizontalAttackPoint;
    [SerializeField] private Transform grenadeLaunchAttackPoint;

        void Start()
    {
        bodyCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        RayCastCheck();
        if (canSeePlayer && !horizontalAttackInProgress && !grenadeAttackInProgress)
        {
            //StartCoroutine(HorizontalAttackPattern());
            //StartCoroutine(GrenadeAttackPattern());
        }
    }

    void RayCastCheck()
    {
        RaycastHit2D playerHit = Raycast(new Vector2(0, -1), Vector2.left, 10);
        if (playerHit)
        {
            canSeePlayer = true;
        }
        else
        {
            canSeePlayer = false;
        }
    }
    
    void HorizontalAttack()
    { 
        Instantiate(bulletPrefab, horizontalAttackPoint.position, Quaternion.identity, null);
    }

    void GrenadeAttack()
    {
        GameObject tempGrenade = Instantiate(grenadePrefab, grenadeLaunchAttackPoint.position, Quaternion.identity, null);
        tempGrenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1), ForceMode2D.Impulse);
    }

    IEnumerator HorizontalAttackPattern()
    {
        horizontalAttackInProgress = true;
        HorizontalAttack();
        yield return new WaitForSeconds(1.0f);
        HorizontalAttack();
        yield return new WaitForSeconds(1.0f);
        HorizontalAttack();
        yield return new WaitForSeconds(2.0f);
        horizontalAttackInProgress = false;
    }
    
    IEnumerator GrenadeAttackPattern()
    {
        grenadeAttackInProgress = true;
        GrenadeAttack();
        yield return new WaitForSeconds(1.0f);
        GrenadeAttack();
        yield return new WaitForSeconds(1.0f);
        GrenadeAttack();
        yield return new WaitForSeconds(2.0f);
        grenadeAttackInProgress = false;
    }
    
    



    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, groundLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the enemy's position
        Vector2 pos = transform.position;

        //Send out the desired raycast and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //If we want to show debug raycasts in the scene...
        if (drawDebugRaycasts)
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        //Return the results of the raycast
        return hit;
    }
}
    

