using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    static GameManager current;
    bool isDead;

    RemovableFloor lockedFloor;

    // Start is called before the first frame update
    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        // UIManager.Update()..?
    }


    public static bool IsGameOver()
    {
        if (current == null)
            return false;

        return current.isDead;
    }


    public static void PlayerDied()
    {
        if (current == null)
            return;

        
    }


    public static void RegisterFloor(RemovableFloor floor)
    {
        if (current == null)
            return;

        current.lockedFloor = floor;
    }
}
