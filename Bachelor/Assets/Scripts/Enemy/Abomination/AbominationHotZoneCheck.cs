using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbominationHotZoneCheck : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private bool playerInside;

    [SerializeField]
    private AbominationMiniBoss enemy;

    [SerializeField]
    private GameObject wall;

    private UnityAction abominationDeadListener;

    private void Awake()
    {
        abominationDeadListener = new UnityAction(SetWallInactive);
    }


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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_NAME))
        {
            playerInside = false;
            enemy.SetInsideHotZone(playerInside);    
        }
    }

    IEnumerator WallDelay()
    {
        yield return new WaitForSeconds(.2f);
        wall.SetActive(true);
    }


    private void OnEnable()
    {
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
