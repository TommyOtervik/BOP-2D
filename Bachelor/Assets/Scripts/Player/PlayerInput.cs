using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//We first ensure this script runs before all other player scripts to prevent laggy
//inputs
[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

    [HideInInspector] public float horizontal;      //Float that stores horizontal input
    [HideInInspector] public bool jumpHeld;         //Bool that stores jump held
    [HideInInspector] public bool jumpPressed;      //Bool that stores jump pressed
    [HideInInspector] public bool crouchHeld;       //Bool that stores crouch held
    [HideInInspector] public bool crouchPressed;    //Bool that stores crouch pressed
    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool altFirePressed;
    [HideInInspector] public bool rangedAttack;
    
    

    bool readyToClear;								//Bool used to keep input in sync
    // Update is called once per frame
    void Update()
    {
        //Clear out existing input values
        ClearInput();
        
        //If the Game Manager says the game is over, exit
        //if (GameManager.IsGameOver())
            //return;

        //Process keyboard, mouse inputs
        ProcessInputs();
        

        //Clamp the horizontal input to be between -1 and 1
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
    }

    void FixedUpdate()
    {
        //In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
        //next Update(). This ensures that all code gets to use the current inputs
        readyToClear = true;
    }

    void ClearInput()
    {
        //If we're not ready to clear input, exit
        if (!readyToClear)
            return;

        //Reset all inputs
        horizontal = 0f;
        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;
        firePressed = false;
        altFirePressed = false;
        rangedAttack = false;
        readyToClear = false;
    }

    void ProcessInputs()
    {
        //Accumulate horizontal axis input
        horizontal += Input.GetAxis("Horizontal");

        //Accumulate button inputs
        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");

        crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
        crouchHeld = crouchHeld || Input.GetButton("Crouch");

        firePressed = firePressed || Input.GetButtonDown("Fire1");
        altFirePressed = altFirePressed || Input.GetButton("Fire2");
        rangedAttack = rangedAttack || Input.GetButton("Fire3");
    }

  
}
