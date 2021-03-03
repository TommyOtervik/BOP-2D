using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private bool inRange;
    private Animator anim; // Kjedelig referanse?

    private const string PLAYER_NAME = "Player";
    private const string FLIP_CULTIST_KEY = "FlipCultist";
    private const string HOT_ZONE_EXIT_KEY = "HotZoneExit";

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (inRange)
        {
            EventManager.TriggerEvent(FLIP_CULTIST_KEY);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_NAME))
        {
            inRange = false;
            gameObject.SetActive(false);

            EventManager.TriggerEvent(HOT_ZONE_EXIT_KEY);
        }
    }

}
