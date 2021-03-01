using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    PlayerCombat player;
    public Slider slider;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCombat>();

    }


    private void Update()
    {
        DrawHealthbar(player.currentHealth);
    }

    private void DrawHealthbar(int currentHealth)
    {
        slider.value = currentHealth;
    }

    public void SetMaxHealth()
    {
        slider.maxValue = player.maxHealth;
        slider.value = player.currentHealth;
    }
}
