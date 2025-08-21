using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : BaseEnemyMovement
{
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private PlayerDetection playerDetection;
    [SerializeField] private bool isAwake = false;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDelay;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool attackLocked = false;
    [SerializeField] private float chaseRange;

    private Rigidbody2D rb;

    private void Start()
    {
        targetPlayer = GameObject.Find("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void WakeUp()
    {
        isAwake = true;
        canAttack = true;
        attackLocked = false;
        
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAwake", true);
            // Don't set IsMoving yet - it will be set when actually chasing
        }
        
        // Ensure the NavMeshAgent is properly configured
        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = 0; // Start stopped, will be set when chasing
        }
    }

    public void Sleep()
    {
        isAwake = false;
        canAttack = false;
        attackLocked = false;
        
        // Stop all movement when sleeping
        StopMovement();
        
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAwake", false);
        }
    }

    void FixedUpdate()
    {
        if (!isAwake) return;
        
        if (targetPlayer != null && !attackLocked)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= attackRange)
            {
                // Stop moving and attack
                StopMovement();
                StartAttack();
            }
            else if (distanceToPlayer <= chaseRange)
            {
                // Chase the player
                if (agent != null)
                {
                    agent.speed = speed;
                    agent.SetDestination(targetPlayer.position);
                }
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetBool("IsMoving", true);
                }
            }
            else
            {
                // Player too far, stop moving
                StopMovement();
            }
        }
    }

    private void StopMovement()
    {
        // Completely stop all movement
        if (agent != null)
        {
            agent.speed = 0;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
        
        // Stop the moving animation
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsMoving", false);
        }
        
        // Stop any rigidbody movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void StartAttack()
    {
        if (canAttack)
        {
            StopMovement();
            
            canAttack = false;
            attackLocked = true;

            if (playerDetection != null)
            {
                playerDetection.Sleep(true, false);
            }
        }
    }
}

