using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    private Hashtable objects = new Hashtable();


    private void Awake()
    {
      var objectsOfInterface = FindObjectsOfType<MonoBehaviour>().OfType<ICanBeSetInactive>();

        foreach (ICanBeSetInactive obj in objectsOfInterface)
        {
            objects.Add(obj.GetObjectName(), obj);           
        }


        foreach (DictionaryEntry ob in objects)
        {
            string name = ob.Key.ToString();

            if (!GameManager.persistenceDictionary.ContainsKey(name))
            {
                Debug.Log("Yo break");
                break;
            }
            

            bool check;
            if (GameManager.persistenceDictionary.TryGetValue(name, out check))
            {
                if (!check)
                {
                    GameObject gameObject = (GameObject) ob.Value;
                   
                    gameObject.SetActive(false);
                }
                    
            }
        }
    }
}
