using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tilhører spilleren.
 *  Håndterer input
 *   
 */


// Vi sørger først for at dette skriptet kjører før alle andre spillerskript for å forhindre laggy inputs
[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

    [HideInInspector] public float horizontal;      // Float som lagrer horisontal input
    [HideInInspector] public bool jumpHeld;         // Bool som lagrer om man holder inne (Hopp)
    [HideInInspector] public bool jumpPressed;      // Bool som lagrer rom man trykker    (Hopp)
    [HideInInspector] public bool firePressed;      // Bool som lagrer rom man trykker
    [HideInInspector] public bool altFirePressed;   // Bool som lagrer rom man trykker
    [HideInInspector] public bool rangedAttack;     // Bool som lagrer rom man trykker
    
    bool readyToClear;								// Bool brukes til å holde synkronisering av inndata

    void Update()
    {
        // Fjern eksisterende inputverdier
        ClearInput();

        // Prosesser tastatur og mus innputs
        ProcessInputs();

        // Fest den horisontale inputen til å være mellom -1 og 1
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
    }

    void FixedUpdate()
    {
        //I FixedUpdate() setter vi et flagg som lar inputs tømmes under
        // neste Update(). Dette sikrer at all kode får bruke de nåværende inputene
       readyToClear = true;
    }

    void ClearInput()
    {
        // Hvis vi ikke er klare til å fjerne inndata, kan du avslutte
        if (!readyToClear)
            return;

        // Tilbakestill alle inputs
        horizontal = 0f;
        jumpPressed = false;
        jumpHeld = false;
        firePressed = false;
        altFirePressed = false;
        rangedAttack = false;
        readyToClear = false;
    }

    void ProcessInputs()
    {
        // Akkumulere horisontal-input
        horizontal += Input.GetAxis("Horizontal");

        // Akkumulere knapp-inputs
        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");


        firePressed = firePressed || Input.GetButtonDown("Fire1");
        altFirePressed = altFirePressed || Input.GetButton("Fire2");
        rangedAttack = rangedAttack || Input.GetButton("Fire3");
    }

  
}
