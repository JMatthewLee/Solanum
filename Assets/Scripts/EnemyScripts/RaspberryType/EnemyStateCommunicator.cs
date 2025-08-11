using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateCommunicator : MonoBehaviour
{
    [SerializeField] GameObject enemy; //Reference to current attached enemy
    [SerializeField] Animator enemyAnimator; //Reference to the animator on current enemy
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private bool isAwake = false;

    void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (playerCollider.CompareTag("Player")) // || playerCollider.CompareTag("PlayerProjectiles")
        {
            WakeUp();
        }
    }

    public void WakeUp() //Function used to Wake up the Enemy and/or stop the Sleep Countdown
    {
        if (!isAwake)
        {
            isAwake = true;
            enemyController.StartAI(); //Enable enemy AI script
        }
    }

    public void IsKilled()
    {
        isAwake = false;
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsKilled", true);
            enemyAnimator.SetBool("IsAwake", false); //Play Enemy Sleep Animation
        }
    }

    //accessedby an animation event
    public void Death()
    {
        enemyController.Death(); //Check this out maybe add more
    }

    public void attackFinish()
    {
        enemyController.attackFinish();
    }
}