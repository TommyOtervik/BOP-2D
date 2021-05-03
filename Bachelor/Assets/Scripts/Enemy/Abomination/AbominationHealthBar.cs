﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Dette skriptet håndterer det visuelle med "Healthbar" til 
 * abomination. 
 * 
 * @ AOP - 225280
 */
public class AbominationHealthBar : MonoBehaviour
{

    [SerializeField]
    private Transform bar;
 
    public void SetSize(float sizeNormalized)
    {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }
}
