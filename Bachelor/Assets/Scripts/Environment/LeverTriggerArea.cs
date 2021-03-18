using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTriggerArea : MonoBehaviour
{

    [SerializeField]
    private RemovableFloor removableFloor;

    private bool insideArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && insideArea)
        {
            Debug.Log("Pressed E");
            removableFloor.Open();

            // Send event? Spilleren 
            
        }
    }

  


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Debug.Log("Player inside");
            insideArea = true;
            UIManager.DisplayEButtonOnLever();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Debug.Log("Player outside");
            insideArea = false;
            UIManager.StopDisplayEButton();
        }
    }
}
