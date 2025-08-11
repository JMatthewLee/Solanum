using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : BaseEnemyMovement
{
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private PlayerDetection playerDetection;
    
    [SerializeField] private bool isAwake;
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
    }

    private void OnEnable()
    {
        //Enable Rigidbody 2D
        rb = GetComponent<Rigidbody2D>();

        isAwake = false;
        canAttack = false;

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsAwake", true); //Play Enemy Wake Up Animation  
        }
    }

    public void WakeUp() //Function refernced in blueberry comunicator
    {
        isAwake = true;
        canAttack = true;
        attackLocked = false;
    }

    public void Sleep() //Function refernced in Player Detection
    {
        isAwake = false;
        canAttack = false;
        attackLocked = false;
    }

    void FixedUpdate()
    {
        if (targetPlayer != null && isAwake && !attackLocked)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= attackRange)
            {
                agent.speed = 0; //stop any movement
                StartAttack();
            }
            else if (distanceToPlayer <= chaseRange)
            {
                agent.speed = speed; //Set Speed to desired value
                ChasePlayer();
            }
            else
            {
                enemyAnimator.SetBool("IsMoving", false);
            }
        }
    }

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
        enemyAnimator.SetBool("IsMoving", true);
    }

    void StartAttack()
    {
        if (canAttack)
        {
            //Debug.Log("attackinitiated");
            enemyAnimator.SetBool("IsMoving", false);
            canAttack = false;
            attackLocked = true;

            playerDetection.Sleep(true, false); //ONLY USED TO EXPLODE BLUEBERRY SINCE ATTACK AND DEATH ARE THE SAME

            //ATTACK LOGIC
            //Consider using an animation event

            //inRangeLocation = (transform.position).normalized; //CAN BE USED TO FIND PLAYER LAST LOCATION
            //lastTargetLocation = (targetPlayer.position).normalized; //CAN BE USED TO FIND PLAYER LAST LOCATION

            //StartCoroutine(AttackDelay()); //ATTACK DELAY USED ON OTHER ENEMIES
        }
    }
}

