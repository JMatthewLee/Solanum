using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private float sleepDelay;

    void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (playerCollider.CompareTag("Player"))
        {
            WakeUp();
        }
    }

    public void WakeUp()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAwake", true);
            // Don't set IsMoving yet - wait for animation to finish
        }
        
        // Don't enable EnemyAI yet - wait for animation event
        // The BlueBerryCommunicator.WakeUpFinish() will handle this
    }

    public void Sleep(bool isExploding, bool isKilled)
    {
        if (enemyAnimator != null)
        {
            if (isExploding)
            {
                enemyAnimator.SetBool("IsExploding", true);
                if(isKilled)
                {
                    enemyAnimator.SetBool("IsKilled", true);
                }
            }
            enemyAnimator.SetBool("IsAwake", false);
        }

        if (enemyAI != null)
        {
            enemyAI.Sleep();
            // Disable the EnemyAI script component when sleeping
            enemyAI.enabled = false;
        }
    }
}
