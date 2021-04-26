using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTriggerArea : MonoBehaviour, ICanBeSetInactive
{
    private const string PLAYER_NAME = "Player";

    [SerializeField]
    private string objectName;

    [SerializeField]
    private RemovableFloor removableFloor;

    private bool insideArea;

    
    void Update()
    {
        // FIXME: Events? 
        if (Input.GetKeyDown(KeyCode.E) && insideArea)
        {
            removableFloor.Open();
        }
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

    public string GetObjectName()
    {
        return objectName;
    }
}
