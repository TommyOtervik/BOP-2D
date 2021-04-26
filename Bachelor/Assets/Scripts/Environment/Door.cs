using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{



    void Start()
    {

        GameManager.RegisterDoor(this);
    }


    public void Open()
    {
        this.gameObject.SetActive(false);
    }

}
