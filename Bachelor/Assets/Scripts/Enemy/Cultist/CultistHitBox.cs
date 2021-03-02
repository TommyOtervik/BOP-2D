using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistHitBox : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    [SerializeField]
    private int damageAmount;

    private Player player;

     private void Awake()
    {
        player = FindObjectOfType<Player>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == PLAYER_NAME)
        {
            // Hvis sant, gjør skade til spilleren
            player.TakeDamage(damageAmount);
        }
    }
}
