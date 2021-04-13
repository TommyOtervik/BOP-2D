using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHotZoneCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private bool inRange;
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
            inRange = true;
            enemy.SetInsideHotZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_NAME))
        {
            inRange = false;
            // gameObject.SetActive(false);
            enemy.SetTriggerArea(true);
            enemy.SetInRange(false);
            enemy.SetInsideHotZone(false);
            // enemy.SelectTarget(); Reset? Gå tilbake?
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
