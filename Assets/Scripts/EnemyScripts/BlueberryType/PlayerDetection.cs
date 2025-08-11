using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] GameObject enemy; //Reference to current attached enemy
    [SerializeField] Animator enemyAnimator; //Reference to the animator on current enemy
    [SerializeField] private EnemyAI enemyAI;

    [SerializeField] private float sleepDelay; //How long the enemy will stay awake once out of detection range
    [SerializeField] private bool isAwake = false;

    private void Start()
    {
        //Debug.Log($"enemy: {enemy != null}");
        //Debug.Log($"enemyAnimator: {enemyAnimator != null}");
        //Debug.Log($"enemyAI: {enemyAI != null}");
    }

    void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (playerCollider.CompareTag("Player")) // || playerCollider.CompareTag("PlayerProjectiles")
        { // Increment the counter when something enters the detection range

            ResetEnemyAwakeStatus();
        }
    }

   // void OnTriggerExit2D(Collider2D playerCollider)
    //{
    //    if (playerCollider.CompareTag("Player")) //|| playerCollider.CompareTag("PlayerProjectiles")
    //    {  // Decrement the counter when something leaves the detection range

    //        RestartEnemySleepCountdown();
    //    }
    //}

    public void ResetEnemyAwakeStatus() //Function used to Wake up the Enemy and/or stop the Sleep Countdown
    {
        if (!isAwake)
        {
            Debug.Log("Player Detected");
            WakeUp();
        }

        /*if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine); // Stop sleep timer if already running
            detectionCoroutine = null;
        }*/
    }

    public void RestartEnemySleepCountdown()
    {
        /*if (detectionCount <= 0 && isAwake)
        {
            detectionCoroutine = StartCoroutine(GoToSleepAfterDelay()); // Start sleep timer if no more objects are in range
        }*/
    }


    /*IEnumerator GoToSleepAfterDelay() //Coroutine for sleep delay
    {
        yield return new WaitForSeconds(sleepDelay);
        if (detectionCount <= 0) // Ensure no objects have re-entered the range
        {
            Sleep(false,false);
        }
    }*/

    void WakeUp()
    {
        isAwake = true;
        Debug.Log("Enemy has woken up!");
        enemyAI.enabled = true; //Enable enemy AI script
    }

    public void Sleep(bool isExploding,bool isKilled)
    {
        isAwake = false;
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
            enemyAnimator.SetBool("IsAwake", false); //Play Enemy Sleep Animation
        }
        Debug.Log("Enemy has gone to sleep!");

        enemyAI.Sleep();
        enemyAI.enabled = false; //Disable enemy AI script
    }
}
