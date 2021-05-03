using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
/*
 * Tilhører Player.
 *   Setter det visuelle i UI basert på helsen til spilleren.
 *   Denne skulle gått gjennom UIManager.
 *   
 *   @AOP - 225280
 */
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private int currentHealth;

    private void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = currentHealth;
    }

    private void SetHealth(int health)
    {
        slider.value = health;
        currentHealth = health;
    }


    private void OnEnable()
    {
        Player.UpdateHealth += SetHealth;
        Player.SetMaxHealth += SetMaxHealth;
    }

    private void OnDisable()
    {
        Player.UpdateHealth -= SetHealth;
        Player.SetMaxHealth -= SetMaxHealth;
    }

}
