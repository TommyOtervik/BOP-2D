using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    static GameManager current;
    private Door lockedDoor;
    private List<PickupKey> keys;
    private RemovableFloor removableFloor;


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

    public static void RegisterInsideTrigger(bool isInside)
    {
        if (current == null)
            return;

        if (current.keys.Count == 1 && isInside)
            current.lockedDoor.Open();
    }

    public static void RegisterDoor(Door door)
    {
        if (current == null)
            return;

        current.lockedDoor = door;
    }
    
    public static void PlayerGrabbedKey(PickupKey key)
    {
        if (current == null)
            return;

        if (!current.keys.Contains(key))
            current.keys.Add(key);
    }

    //public static void RegisterAbomination(AbominationMiniBoss abom)
    //{
    //    if (current == null)
    //        return;

    //    current.abom = abom;  
    //}


    public static void PlayerOpenedFloor(bool isOpen)
    {
        current.hasBeenOpened = isOpen;
    }

    public static bool IsFloorOpen()
    {
        return current.hasBeenOpened;
    }


    public static void PlayerKilledAbom(bool isKilled)
    {
        current.hasBeenKilled = isKilled;
    }

    public static bool IsAbomDead()
    {
        return current.hasBeenKilled;
    }
}
