using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dette skirptet håndterer TriggerArea til "RemovableFloor".
 * 
 *   
 * @AOP - 225280
 */
public class LeverTriggerArea : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    [SerializeField] private RemovableFloor removableFloor; // Referanse til gulvet
    private bool insideArea; // Om spiller er innenfor området

    void Update()
    { 
        // Trykker spilleren 'E' og hen er på insiden -> Åpne gulvet
        if (Input.GetKeyDown(KeyCode.E) && insideArea)
            removableFloor.Open();  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_NAME))
        {
            insideArea = true;
            UIManager.DisplayEButtonOnLever();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER_NAME))
        {
            insideArea = false;
            UIManager.StopDisplayEButton();
        }
    }

}
