using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private bool inRange;
    private const string PLAYER_NAME = "Player";
    private EnemyCultist enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyCultist>();
    }

    private void Update()
    {
        if(inRange && enemyParent.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            enemyParent.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            inRange = false;
            gameObject.SetActive(false);
            enemyParent.SetTriggerArea(true);
            enemyParent.SetInRange(false);
            enemyParent.SelectTarget();
        }
    }

}
