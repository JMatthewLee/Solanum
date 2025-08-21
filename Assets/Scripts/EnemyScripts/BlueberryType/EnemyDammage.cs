using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDammage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private LayerMask playerLayer = 1;
    
    private bool hasDealtDamage = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;
        
        if (other.CompareTag("Player"))
        {
            // Deal damage to player
            PlayerValues playerValues = other.GetComponent<PlayerValues>();
            if (playerValues != null)
            {
                playerValues.RecieveDamage(damageAmount);
                hasDealtDamage = true;
            }
        }
    }

    void OnEnable()
    {
        hasDealtDamage = false;
    }
}