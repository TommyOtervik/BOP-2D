using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private Player player;


    private void Start()
    {
        player = FindObjectOfType<Player>();
        SetMaxHealth();
    }

    private void Update()
    {
        SetHealth(player.GetCurrentHealth());
    }

    public void SetMaxHealth()
    {
        slider.maxValue = player.GetMaxHealth();
        slider.value = player.GetCurrentHealth();
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

 
}
