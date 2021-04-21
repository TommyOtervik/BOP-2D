using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupBroker
{
    public static event Action<int> HealthPickupEvent;
    public static event Action<int> CoinPickupEvent;

    public static event Action KeyPickupEvent;
    
    
    public static void CallHealthPickupEvent(int healthValue)
    {
        HealthPickupEvent?.Invoke(healthValue);
    }
    
    public static void CallCoinPickupEvent(int coinValue)
    {
        CoinPickupEvent?.Invoke(coinValue);
    }

    public static void CallKeyPickupEvent()
    {
        KeyPickupEvent?.Invoke();
    }

}