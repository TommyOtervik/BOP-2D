using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    // Kjdelig referanse til cultist..
    private EnemyCultist enemyParent;
 

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyCultist>();

    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            // Skulle ha sendt data på en annen måte.
            // Ikke public metoder..?
            enemyParent.SetTarget(collider.transform);
            enemyParent.SetInRange(true);
            enemyParent.GetHotZone().SetActive(true);
        }
    }
}
