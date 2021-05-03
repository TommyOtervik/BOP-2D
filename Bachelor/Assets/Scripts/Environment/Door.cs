using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Dette skriptet tilhører dør objektet som orginalt er låst,
 * reigstrers is GameManager.
 *   
 * @AOP - 225280
 */
public class Door : MonoBehaviour
{
    void Start()
    {

        GameManager.RegisterDoor(this);
    }

    
    // Setter objektet inaktivt. ("Åpner døren")
    public void Open()
    {
        this.gameObject.SetActive(false);
    }

}
