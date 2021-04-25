using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalModel : MonoBehaviour
{
    private string objectName { get; set; }
    private bool isEnabled { get; set; }


    public EnvironmentalModel(string objectName, bool isEnabled)
    {
        this.objectName = objectName;
        this.isEnabled = isEnabled;
    }



}
