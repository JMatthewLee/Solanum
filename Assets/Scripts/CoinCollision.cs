using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    public int value;
    public PlayerValues playerValueCoins;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is in the "Destroyable" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerValueCoins.GainCoins(value);
            Destroy(gameObject);
        }
    }
}