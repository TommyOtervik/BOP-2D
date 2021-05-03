using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Dette skriptet sjekker om spilleren er innen for en sone, og 
 * håndterer aktivering/deaktivering av veggen.
 * 
 * @ AOP - 225280
 */
public class AbominationHotZoneCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private bool playerInside;

    [SerializeField] private AbominationMiniBoss enemy;
    [SerializeField] private GameObject wall;

    // Lytter for å deaktivere veggen i "boss-rommet"
    private UnityAction abominationDeadListener;

    private void Awake()
    {
        abominationDeadListener = new UnityAction(SetWallInactive);
    }


    // Sjekker om spilleren er innenfor sonen, aktiverer veggen bak.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME) && !enemy.GetIsDead())
        {
            playerInside = true;
            enemy.SetInsideHotZone(playerInside);
            enemy.SetEnrageTimer(5f);
            StartCoroutine(WallDelay());
        }
    }

    // Hvis spilleren kommer seg ut i fra sonen (skal være umulig, men man kan ikke være for sikker!)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_NAME))
        {
            playerInside = false;
            enemy.SetInsideHotZone(playerInside);    
        }
    }

    // Setter litt forsinkelse på aktivering av vegg,
    // pga. spilleren ikke skal ta skade når hen går innenfor området.
    IEnumerator WallDelay()
    {
        yield return new WaitForSeconds(.2f);
        wall.SetActive(true);
    }

   
    private void OnEnable()
    {
        // Blir brukt for å deaktivere veggen
        EventManager.StartListening(EnumEvents.ABOMINATION_DEAD, abominationDeadListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.ABOMINATION_DEAD, abominationDeadListener);
    }


    private void SetWallInactive()
    {
        this.wall.SetActive(false);
    }
}
