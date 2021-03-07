using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private Rigidbody2D enemyRb;
    private bool tickInProgress;
    
    // Endre dette later, virker snodig å ha player referanse i alt som kan skade player. 
    private const string PLAYER_NAME = "Player";
    
    private int damageAmount = 20;

    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator TickTick()
    {
        tickInProgress = true;
        yield return new WaitForSeconds(0.2f);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == PLAYER_NAME)
        {
            // Hvis sant, gjør skade til spilleren
            player.TakeDamage(damageAmount);
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        } 
        
        else if (!tickInProgress) 
        {
            StartCoroutine(TickTick());
        }
        
        
    }
    
    


}

