using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public float speed, jumpHeight, maxHealth, health, maxMana, mana, damage, timeBetweenSwings;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    void TakeDamage (float damage)
    {
        health -= damage;
    }
}
