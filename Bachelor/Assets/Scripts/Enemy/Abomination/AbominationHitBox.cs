using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHitBox : MonoBehaviour
{

    private const string PLAYER_NAME = "Player";

    [SerializeField]
    private int damageAmount = 30;


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
            DamageBroker.CallTakeDamageEvent(damageAmount);
    }

    private void DisableThis()
    {
        GetComponent<BoxCollider2D>().gameObject.SetActive(false);
    }


}
