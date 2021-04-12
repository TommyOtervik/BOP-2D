using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    static GameManager current;
  
    //public static Dictionary<string, bool> persistenceDictionary = new Dictionary<string, bool>();

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

    //public static void AddToPersistenceDictionary(string key, bool value)
    //{

    //    persistenceDictionary.Add(key, value);
    //}


    


}
