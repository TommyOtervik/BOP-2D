using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CultistHitBox : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private readonly int damageAmount = 20;

    public static event Action<int> CultistDamage;
    private UnityAction cultistDeadListener;


    private void Awake()
    {
        cultistDeadListener = new UnityAction(DisableThis);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            CultistDamage?.Invoke(damageAmount);
        }
    }

    private void DisableThis()
    {
         GetComponent<BoxCollider2D>().gameObject.SetActive(false);   
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.CULTIST_DEAD, cultistDeadListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.CULTIST_DEAD, cultistDeadListener);
    }
}
