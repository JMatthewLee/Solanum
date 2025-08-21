using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueberryExplosion : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private LayerMask playerLayer = 1;
    
    private bool hasExploded = false;

    void Start()
    {
        // Start explosion after a short delay
        StartCoroutine(ExplodeAfterDelay(0.1f));
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Find player in explosion radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Damage the player
                PlayerValues playerValues = hitCollider.GetComponent<PlayerValues>();
                if (playerValues != null)
                {
                    playerValues.RecieveDamage(explosionDamage);
                }
            }
        }

        // Destroy explosion after a short time
        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        // Show explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
