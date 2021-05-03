using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
 * Statisk klasse som holder styr på permanente endringer i spillet.
 * 
 */
public class GameManager : MonoBehaviour
{

    static GameManager current;  

    private Door lockedDoor; 
    private List<PickupKey> keys;
    

    //private AbominationMiniBoss abom;

    // Om abom. er drept
    private bool hasBeenKilled;
    // Om gulv er tatt bort
    private bool hasBeenOpened;

    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        keys = new List<PickupKey>();

        DontDestroyOnLoad(gameObject);
    }

    // Sjekker om spilleren har nøkkelen og er på innsiden av et område.
    public static void RegisterInsideTrigger(bool isInside)
    {
        if (current == null)
            return;

        if (current.keys.Count == 1 && isInside)
            current.lockedDoor.Open();
    }

    // Registerer at døren eksisterer
    public static void RegisterDoor(Door door)
    {
        if (current == null)
            return;

        current.lockedDoor = door;
    }
    
    // Registerer at spilleren fikk nøkkelen
    public static void PlayerGrabbedKey(PickupKey key)
    {
        if (current == null)
            return;

        if (!current.keys.Contains(key))
            current.keys.Add(key);
    }

    // Registerer at gulvet er åpnet
    public static void PlayerOpenedFloor(bool isOpen)
    {
        current.hasBeenOpened = isOpen;
    }

    public static bool IsFloorOpen()
    {
        return current.hasBeenOpened;
    }

    // Registrerer at spilleren har drept Abom.
    public static void PlayerKilledAbom(bool isKilled)
    {
        current.hasBeenKilled = isKilled;
    }

    public static bool IsAbomDead()
    {
        return current.hasBeenKilled;
    }
}
