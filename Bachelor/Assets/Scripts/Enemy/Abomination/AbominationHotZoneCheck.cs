using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHotZoneCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private bool playerInside;

    [SerializeField]
    private AbominationMiniBoss enemy;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            playerInside = true;
            enemy.SetInsideHotZone(playerInside);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_NAME))
        {
            playerInside = false;
            enemy.SetInsideHotZone(playerInside);
           
            // enemy.SelectTarget(); Reset? Gå tilbake?
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
