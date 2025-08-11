using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseEnemyMovement
{
    public static event Action<EnemyController> EnemyHasDied; //BRODCAST THIS ENEMY HAS DIED

    public static List<EnemyController> allControllerEnemies = new List<EnemyController>(); //Used to BroadCast Trigger Functions (E.G trigger to wakeup all enemies)

    [SerializeField] private EnemyStateCommunicator enemyStateCommunicator; //reference to the player detection script used to call the sleep function
    [SerializeField] private DammageFlash dammageFlash;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private PlayerValues playerValueHp;

    [SerializeField] private float enemyMaxHp;
    [SerializeField] private float enemyCurrentHp;
    [SerializeField] private float speed;
    [SerializeField] private int collisionDammage;
 
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDelay;
    [SerializeField] private float chaseRange;

    private bool isAwake;
    private bool canAttack;
    private bool attackLocked = false;

    private Rigidbody2D rb;

    //----------------------------------------Initializers---------------------------------------------------------------------------------------------

    void OnEnable()
    {
        allControllerEnemies.Add(this);
    }

    void OnDisable()
    {
        allControllerEnemies.Remove(this);
    }

    void Awake()
    {
        //Initialize NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        // Improve enemy avoidance to prevent overlapping
        agent.radius = 0.3f; // Increase personal space
        agent.avoidancePriority = UnityEngine.Random.Range(1, 100); // Random priority to prevent all enemies having same priority
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.autoBraking = true; // Helps with smoother movement
        agent.stoppingDistance = 0.1f; // Small stopping distance to prevent exact overlap
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemyCurrentHp = enemyMaxHp;
        playerValueHp = GameObject.Find("Player").GetComponent<PlayerValues>();
        targetPlayer = GameObject.Find("Player").GetComponent<Transform>();
    }

    //-------------------------------------------AI---------------------------------------------------------------------------------------------------------
    public void StartAI()
    {
        isAwake = true;
        canAttack = true;
        attackLocked = false;
    }

    void FixedUpdate()
    {
        if (targetPlayer != null && isAwake && !attackLocked)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position); //change to give variety

            if (distanceToPlayer <= attackRange) 
            {
                //ADD if statement to check if the raspberry is above water if so continue chasing player at 45 degree angle until there is no water

                agent.speed = 0; //stop any movement
                StartAttack();

                //Add something to make enemy move after each attack
            }
            else if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }

            //Maybe add another condition for chase range to increase speed when far away
        }
    }

    //-----------------------------------CHASE--------------------------------------------------------------------------------------------------------------------------------------

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
        enemyAnimator.SetBool("IsMoving", true);
        agent.speed = speed; //Set Speed to desired value
    }

    //---------------------------------ATTACK----------------------------------------------------------------------------------------------------------------------------------------

    void StartAttack()
    {
        if (canAttack)
        {
            //Debug.Log("attackinitiated");
            enemyAnimator.SetBool("IsMoving", false);
            enemyAnimator.SetBool("IsAttacking", true);
            canAttack = false;
            attackLocked = true;

            //ATTACK LOGIC WILL BE CALLED IN ANIMATION EVENT IN SEPERATE SCRIPT
            //enemyAttackScript.Attack(); LIKE THIS BUT BY ANIMATION EVENT
        }
    }

    public void attackFinish()
    {
        enemyAnimator.SetBool("IsAttacking", false);
        canAttack = true;
        attackLocked = false;
    }

    //--------------------------------DAMAGE RECEIVING------------------------------------------------------------------------------------------------------------------------------------

    public void TakeDamage(float damagetaken) //Maybe add A fixed update to check for DOT (Dmg Over Time) IN THE FUTURE FOR NOW LEAVE IT
    {
        WakeUp();

        enemyCurrentHp -= damagetaken;
        if (enemyCurrentHp <= 0)
        {
            dammageFlash.CallHitFlash();

            isAwake = false;
            canAttack = false;
            attackLocked = false;
            enemyCurrentHp = 0;

            enemyStateCommunicator.IsKilled();
        }
        else
        {
            dammageFlash.CallHitFlash(); //Play some sort of enemy hurt animation (MAYBE JUST ADD A WHITE 30% TO THE CURRENT SPRITE)
            //Play enemy hurt sound (MAYBE LIKE A Squish?)
        }
    }

    public void WakeUp()
    {
        enemyStateCommunicator.WakeUp();
    }

    public void Death()//Called by animation event Destroy the game object once Death animation is finished
    {
        //Maybe ADD A DEATH SFX
        Destroy(gameObject);
        EnemyHasDied?.Invoke(this); //BROADCAST THIS ENEMY HAS DIED
    }
}

