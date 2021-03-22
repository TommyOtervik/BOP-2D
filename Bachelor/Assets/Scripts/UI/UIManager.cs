using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    static UIManager current;

    [SerializeField]
    private GameObject EKey;

    // Start is called before the first frame update
    void Awake()
    {

        //If an UIManager exists and it is not this...
        if (current != null && current != this)
        {
            //...destroy this and exit. There can be only one UIManager
            Destroy(gameObject);
            return;
        }

        //This is the current UIManager and it should persist between scene loads
        current = this;
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // FIXME: IKKE HELT OPTIMALT.. 2 metoder..
    public static void DisplayEButtonOnLever()
    {
        if (current == null)
            return;

        current.EKey.SetActive(true);
    }

    public static void StopDisplayEButton()
    {
        if (current == null)
            return;

        current.EKey.SetActive(false);

    } 
}
