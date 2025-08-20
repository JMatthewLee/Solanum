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
 
    [Header("Attack Pattern Configuration")]
    [SerializeField] private float attackRange; // Range to initiate first attack
    [SerializeField] private float attackDelay;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackContinuationRange = 8f; // Range to continue attacking (larger than attackRange)
    [SerializeField] private float repositionRange = 12f; // Range where enemy will reposition instead of attacking
    [SerializeField] private float maxAttackDistance = 15f; // Maximum distance before enemy gives up and moves
    [SerializeField] private int maxAttacksPerEngagement = 2; // Maximum number of attacks before repositioning
    [SerializeField] private float repositionTime = 3f; // Time to spend repositioning
    [SerializeField] private float strategicPositioningRadius = 4f; // Radius for strategic positioning around player
    [SerializeField] private float movementSpeedMultiplier = 1.5f; // Multiplier for faster movement
    [SerializeField] private float stopThreshold = 0.1f; // Distance threshold to consider stopped
    [SerializeField] private float immediateAttackRange = 2f; // Very close range for immediate attack
    [SerializeField] private float landingCommitTime = 0.5f; // Time to commit to attack after landing
    [SerializeField] private float blueberrySpeedModifier = 0.8f; // Blueberry-specific speed modifier (slower than player)
    [SerializeField] private float projectileAlignmentRadius = 3f; // Radius for positioning to align projectiles with player
    [SerializeField] private float alignmentTolerance = 15f; // Degrees tolerance for projectile alignment
    [SerializeField] private bool isBlueberryEnemy = false; // Manual override to identify blueberry enemies
    [SerializeField] private bool usePatrolMode = true; // Enable patrol mode for independent movement
    [SerializeField] private bool useNodeBasedPatrol = true; // Use node-based patrol instead of random movement
    [SerializeField] private Transform[] patrolNodes; // Array of patrol nodes to follow
    [SerializeField] private float nodeReachThreshold = 1.5f; // Distance to consider node reached
    [SerializeField] private float nodeWaitTime = 0.75f; // Time to wait at each node
    [SerializeField] private float maxDistanceFromNodes = 6f; // Maximum distance enemy can stray from nodes
    [SerializeField] private float tacticalNodeRadius = 4f; // Radius around nodes for tactical positioning
    
    // Legacy patrol variables for backward compatibility
    [SerializeField] private float patrolRadius = 8f; // Radius around spawn point for patrol area (legacy)
    [SerializeField] private float patrolPointChangeTime = 5f; // Time to spend at each patrol point (legacy)

    private bool isAwake;
    private bool canAttack;
    private bool attackLocked = false;

    // Attack pattern state management
    private enum AttackState
    {
        Patrolling,       // Moving around map independently
        Approaching,      // Moving towards player to get in attack range
        Attacking,        // Currently attacking
        Repositioning,    // Moving to a new strategic position
        Returning,        // Moving back towards player after repositioning
        ImmediateAttack   // Immediate attack response for very close threats
    }
    
    private AttackState currentState = AttackState.Approaching;
    private int attacksPerformed = 0;
    private float stateTimer = 0f;
    private Vector3 strategicTargetPosition;
    private bool needsNewStrategicPosition = true;
    private float landingTimer = 0f; // Timer for committing to attack after landing
    private bool hasLanded = false; // Track if enemy has landed/stopped moving
    
    // Patrol system variables
    private Vector3 spawnPosition; // Original spawn position for patrol area
    private Vector3 currentPatrolTarget; // Current patrol destination
    private float patrolTimer = 0f; // Time spent at current patrol point
    private bool needsNewPatrolPoint = true; // Flag to get new patrol point
    
    // Node-based patrol system
    private int currentNodeIndex = 0; // Current node index in patrol sequence
    private Transform currentNode; // Current patrol node
    private bool isMovingToNextNode = true; // Whether we're moving to next node or waiting
    private Vector3 tacticalPosition; // Position near current node for tactical advantage
    private bool needsTacticalPosition = true; // Flag to calculate new tactical position
    
    // Random shooting timer
    private float randomShootTimer = 0f; // Timer for random shooting during movement
    private float randomShootInterval = 2f; // Check for random shooting every 2 seconds
    
    // Attack timing
    private float attackStartTime = 0f; // When the attack started
    private float minimumAttackDuration = 0.5f; // Minimum time an attack must last

    private Rigidbody2D rb;
    
    // Helper method to check if this enemy is a blueberry
    private bool IsBlueberry()
    {
        // Check manual override first, then by name
        if (isBlueberryEnemy) return true;
        
        string objectName = gameObject.name.ToLower();
        return objectName.Contains("blueberry") || 
               objectName.Contains("blue");
    }
    
    // Calculate optimal position for projectile alignment
    private Vector3 GetProjectileAlignmentPosition(Vector3 playerPosition, Vector3 currentPosition, float radius)
    {
        // Get the direction from player to enemy
        Vector3 directionToEnemy = (currentPosition - playerPosition).normalized;
        
        // Calculate the angle from player to enemy
        float currentAngle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
        
        // Find the best alignment angle (multiple of 45 degrees for 8-directional shooting)
        float[] alignmentAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        float bestAngle = alignmentAngles[0];
        float smallestDifference = Mathf.Abs(currentAngle - alignmentAngles[0]);
        
        foreach (float angle in alignmentAngles)
        {
            float difference = Mathf.Abs(currentAngle - angle);
            if (difference > 180f) difference = 360f - difference; // Handle angle wrapping
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                bestAngle = angle;
            }
        }
        
        // Convert the best angle back to radians and calculate position
        float bestAngleRad = bestAngle * Mathf.Deg2Rad;
        Vector3 alignedDirection = new Vector3(Mathf.Cos(bestAngleRad), Mathf.Sin(bestAngleRad), 0f);
        
        // Return position at the specified radius from player
        return playerPosition + (alignedDirection * radius);
    }
    
    // Check if current position is well-aligned for projectile attack
    private bool IsWellAlignedForProjectile(Vector3 playerPosition, Vector3 currentPosition)
    {
        Vector3 directionToEnemy = (currentPosition - playerPosition).normalized;
        float currentAngle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
        
        // Check if we're close to any of the 8-directional angles
        float[] alignmentAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        
        foreach (float angle in alignmentAngles)
        {
            float difference = Mathf.Abs(currentAngle - angle);
            if (difference > 180f) difference = 360f - difference; // Handle angle wrapping
            if (difference <= alignmentTolerance)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Get the next patrol node randomly
    private Transform GetNextPatrolNode()
    {
        if (patrolNodes == null || patrolNodes.Length == 0)
        {
            Debug.LogWarning($"Enemy {gameObject.name}: No patrol nodes assigned!");
            return null;
        }
        
        // Choose a random node (avoiding the current one if possible)
        int randomIndex;
        if (patrolNodes.Length > 1)
        {
            // If we have multiple nodes, avoid choosing the current one
            do
            {
                randomIndex = UnityEngine.Random.Range(0, patrolNodes.Length);
            } while (randomIndex == currentNodeIndex);
        }
        else
        {
            // If we only have one node, just use it
            randomIndex = 0;
        }
        
        currentNodeIndex = randomIndex;
        return patrolNodes[currentNodeIndex];
    }
    
    // Get current patrol node
    private Transform GetCurrentPatrolNode()
    {
        if (patrolNodes == null || patrolNodes.Length == 0)
        {
            Debug.LogWarning($"Enemy {gameObject.name}: No patrol nodes assigned!");
            return null;
        }
        
        return patrolNodes[currentNodeIndex];
    }
    
    // Calculate tactical position near current node for better attack angles
    private Vector3 GetTacticalPositionNearNode(Transform node, Vector3 playerPosition)
    {
        if (node == null) return transform.position;
        
        Vector3 nodePosition = node.position;
        Vector3 directionToPlayer = (playerPosition - nodePosition).normalized;
        
        // Get tactical position that's well-aligned for projectiles
        Vector3 tacticalPos = GetProjectileAlignmentPosition(playerPosition, nodePosition, tacticalNodeRadius);
        
        // Ensure the tactical position is reachable
        if (!IsPositionReachable(tacticalPos))
        {
            // Fallback to a position near the node
            tacticalPos = GetFallbackPosition(nodePosition, tacticalNodeRadius * 0.5f);
        }
        
        return tacticalPos;
    }
    
    // Check if we should stay near current node (when player is spotted)
    private bool ShouldStayNearCurrentNode(float distanceToPlayer)
    {
        if (currentNode == null) return false;
        
        // If player is within attack range, stay near current node for tactical advantage
        if (distanceToPlayer <= attackRange * 1.5f)
        {
            return true;
        }
        
        // If player is very close, definitely stay near node
        if (distanceToPlayer <= immediateAttackRange * 2f)
        {
            return true;
        }
        
        return false;
    }
    
    // Generate a new patrol point within the patrol area (fallback for non-node patrol)
    private Vector3 GetNewPatrolPoint()
    {
        // Get a random angle
        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        // Get a random distance within patrol radius
        float randomDistance = UnityEngine.Random.Range(patrolRadius * 0.3f, patrolRadius);
        
        // Calculate patrol point
        Vector3 patrolPoint = spawnPosition + new Vector3(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance,
            0f
        );
        
        // Ensure the point is reachable
        if (!IsPositionReachable(patrolPoint))
        {
            patrolPoint = GetFallbackPosition(spawnPosition, randomDistance);
        }
        
        return patrolPoint;
    }
    
    // Check if we should transition from patrolling to attacking
    private bool ShouldTransitionToAttack(float distanceToPlayer)
    {
        // Only attack if we're at a patrol node and player is in range
        if (currentNode != null && distanceToPlayer <= attackRange)
        {
            // Check if we're close enough to our current node to attack from it
            float distanceToNode = Vector2.Distance(transform.position, currentNode.position);
            if (distanceToNode <= nodeReachThreshold * 1.5f) // Slightly more lenient for attack positioning
            {
                return true;
            }
        }
        
        return false;
    }

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
        
        // Initialize patrol system
        spawnPosition = transform.position;
        needsNewPatrolPoint = true;
        
        // Initialize node-based patrol system
        if (useNodeBasedPatrol && patrolNodes != null && patrolNodes.Length > 0)
        {
            currentNode = patrolNodes[0];
            currentNodeIndex = 0;
            currentPatrolTarget = currentNode.position;
            needsTacticalPosition = true;
        }
    }

    //-------------------------------------------AI---------------------------------------------------------------------------------------------------------
    public void StartAI()
    {
        isAwake = true;
        canAttack = true;
        attackLocked = false;
        currentState = usePatrolMode ? AttackState.Patrolling : AttackState.Approaching;
        Debug.Log($"Enemy {gameObject.name}: Initial state set to {currentState}");
        attacksPerformed = 0;
        needsNewStrategicPosition = true;
        landingTimer = 0f;
        hasLanded = false;
        needsNewPatrolPoint = true;
        patrolTimer = 0f;
        
        // Initialize node-based patrol
        if (useNodeBasedPatrol && patrolNodes != null && patrolNodes.Length > 0)
        {
            currentNode = patrolNodes[0];
            currentNodeIndex = 0;
            currentPatrolTarget = currentNode.position;
            needsTacticalPosition = true;
            isMovingToNextNode = true;
        }
        
        Debug.Log($"Enemy {gameObject.name}: AI Started - State: {currentState}, CanAttack: {canAttack}, PatrolMode: {usePatrolMode}, NodeBased: {useNodeBasedPatrol}");
    }

    void FixedUpdate()
    {
        if (targetPlayer != null && isAwake && !attackLocked)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
            
            // Debug: Log current state and conditions
            if (currentState == AttackState.Attacking)
            {
                Debug.Log($"Enemy {gameObject.name}: In FixedUpdate - State: {currentState}, CanAttack: {canAttack}, AttackLocked: {attackLocked}");
            }
            
            UpdateAttackState(distanceToPlayer);
            ExecuteCurrentState(distanceToPlayer);
            
            // Handle landing commitment timer
            if (hasLanded && currentState != AttackState.Attacking)
            {
                landingTimer += Time.fixedDeltaTime;
                if (landingTimer >= landingCommitTime)
                {
                    // Commit to attack after landing
                    if (canAttack && !attackLocked)
                    {
                        Debug.Log($"Enemy {gameObject.name}: Committing to attack after landing");
                        StartAttack();
                    }
                }
            }
            
            // Handle attack timeout (fallback if animation event never fires)
            if (currentState == AttackState.Attacking && attackLocked)
            {
                float timeSinceAttackStarted = Time.time - attackStartTime;
                if (timeSinceAttackStarted > minimumAttackDuration + 1f) // Give extra 1 second buffer
                {
                    Debug.Log($"Enemy {gameObject.name}: Attack timeout reached ({timeSinceAttackStarted:F2}s), forcing attack finish");
                    attackFinish();
                }
            }
        }
    }

    private void UpdateAttackState(float distanceToPlayer)
    {
        // Check if we need to reset attack count (player moved too far)
        if (distanceToPlayer > maxAttackDistance)
        {
            attacksPerformed = 0;
            currentState = usePatrolMode ? AttackState.Patrolling : AttackState.Approaching;
            needsNewStrategicPosition = true;
            needsNewPatrolPoint = true;
        }
        
        // State transitions based on distance and attack count
        switch (currentState)
        {
            case AttackState.Patrolling:
                // Only attack if we're at a node and player is in range
                if (ShouldTransitionToAttack(distanceToPlayer))
                {
                    // Stay in patrolling state but allow attack from current node
                    // The attack will be handled in PatrolUsingNodes when we reach the node
                }
                // Never transition to Approaching - stay in patrol mode
                break;
                
            case AttackState.Approaching:
                if (distanceToPlayer <= attackRange)
                {
                    Debug.Log($"Enemy {gameObject.name}: Transitioning to Attacking state at distance {distanceToPlayer:F2}");
                    currentState = AttackState.Attacking;
                    stateTimer = 0f;
                    hasLanded = true;
                    landingTimer = 0f;
                    // Ensure we can start attacking immediately
                    canAttack = true;
                    attackLocked = false;
                }
                break;
                
            case AttackState.Attacking:
                // Stay in attacking state until attack is finished
                break;
                
            case AttackState.ImmediateAttack:
                // Stay in immediate attack state until attack is finished
                break;
                
            case AttackState.Repositioning:
                stateTimer += Time.fixedDeltaTime;
                if (stateTimer >= repositionTime)
                {
                    currentState = AttackState.Returning;
                    stateTimer = 0f;
                }
                break;
                
            case AttackState.Returning:
                if (distanceToPlayer <= attackRange)
                {
                    Debug.Log($"Enemy {gameObject.name}: Transitioning to Attacking state from Returning at distance {distanceToPlayer:F2}");
                    currentState = AttackState.Attacking;
                    stateTimer = 0f;
                    hasLanded = true;
                    landingTimer = 0f;
                    // Ensure we can start attacking immediately
                    canAttack = true;
                    attackLocked = false;
                }
                break;
        }
    }

    private void ExecuteCurrentState(float distanceToPlayer)
    {
        switch (currentState)
        {
            case AttackState.Patrolling:
                PatrolAround();
                break;
                
            case AttackState.Approaching:
                ApproachPlayer();
                break;
                
            case AttackState.Attacking:
                // Start the attack if we can
                Debug.Log($"Enemy {gameObject.name}: In ExecuteCurrentState - Attacking case, CanAttack: {canAttack}, AttackLocked: {attackLocked}");
                if (canAttack && !attackLocked)
                {
                    Debug.Log($"Enemy {gameObject.name}: Calling StartAttack() from ExecuteCurrentState");
                    StartAttack();
                }
                break;
                
            case AttackState.ImmediateAttack:
                // Start immediate attack if we can
                if (canAttack && !attackLocked)
                {
                    StartAttack();
                }
                break;
                
            case AttackState.Repositioning:
                RepositionStrategically();
                break;
                
            case AttackState.Returning:
                ReturnToPlayer();
                break;
        }
        
        // Debug logging for state transitions
        if (isAwake && targetPlayer != null && currentState == AttackState.Attacking && canAttack && !attackLocked)
        {
            Debug.Log($"Enemy {gameObject.name}: Ready to attack! Distance={distanceToPlayer:F2}");
        }
    }

    //-----------------------------------MOVEMENT STATES--------------------------------------------------------------------------------------------------------------------------------------
    
    private void PatrolAround()
    {
        if (useNodeBasedPatrol && patrolNodes != null && patrolNodes.Length > 0)
        {
            PatrolUsingNodes();
        }
        else
        {
            PatrolRandomly();
        }
    }
    
    private void PatrolUsingNodes()
    {
        // Check if we need to move to next node
        if (isMovingToNextNode)
        {
            if (currentNode == null)
            {
                currentNode = GetCurrentPatrolNode();
                if (currentNode == null) return;
            }
            
            // Always move directly to the current node (no tactical positioning)
            currentPatrolTarget = currentNode.position;
            
            // Move to current target
            agent.SetDestination(currentPatrolTarget);
            
            // Apply speed modifiers - blueberry gets additional slowdown
            float finalSpeed = speed * movementSpeedMultiplier * 0.6f; // Slower when patrolling
            if (IsBlueberry())
            {
                finalSpeed *= blueberrySpeedModifier;
            }
            agent.speed = finalSpeed;
            enemyAnimator.SetBool("IsMoving", true);
            
            // Random shooting during movement (1/5 chance every 2 seconds)
            randomShootTimer += Time.fixedDeltaTime;
            if (randomShootTimer >= randomShootInterval)
            {
                randomShootTimer = 0f; // Reset timer
                
                if (canAttack && !attackLocked)
                {
                    // Check if we should randomly shoot during movement (regardless of player range)
                    if (UnityEngine.Random.Range(0f, 1f) < 0.2f) // 20% chance (1/5)
                    {
                        // Random shooting during movement
                        currentState = AttackState.Attacking;
                        canAttack = true;
                        attackLocked = false;
                        hasLanded = true;
                        landingTimer = 0f;
                        
                        // Immediately start the attack
                        StartAttack();
                        return; // Exit to let attack state handle it
                    }
                }
            }
            
                         // Check if we've reached the target
             float distanceToTarget = Vector2.Distance(transform.position, currentPatrolTarget);
             if (distanceToTarget <= nodeReachThreshold)
             {
                 // Reached the target, start waiting and check if we should attack
                 StopMovement();
                 isMovingToNextNode = false;
                 patrolTimer = 0f;
                 
                 // Always attack when reaching a node (regardless of player range)
                 if (canAttack && !attackLocked)
                 {
                     // Attack from this node even if player is not in range
                     currentState = AttackState.Attacking;
                     canAttack = true;
                     attackLocked = false;
                     hasLanded = true;
                     landingTimer = 0f;
                     
                     // Immediately start the attack since we're already in the right state
                     Debug.Log($"Enemy {gameObject.name}: Node reached, calling StartAttack()");
                     StartAttack();
                 }
             }
        }
        else
        {
            // Waiting at current node
            patrolTimer += Time.fixedDeltaTime;
            
            if (patrolTimer >= nodeWaitTime)
            {
                // Time to move to next node
                currentNode = GetNextPatrolNode();
                isMovingToNextNode = true;
                needsTacticalPosition = true;
                randomShootTimer = 0f; // Reset random shoot timer for new movement
                
                // Ensure we don't stray too far from nodes
                float distanceFromCurrentNode = Vector2.Distance(transform.position, currentNode.position);
                if (distanceFromCurrentNode > maxDistanceFromNodes)
                {
                    // We've strayed too far, go directly to current node
                    currentPatrolTarget = currentNode.position;
                }
            }
        }
    }
    
    private void PatrolRandomly()
    {
        if (needsNewPatrolPoint)
        {
            currentPatrolTarget = GetNewPatrolPoint();
            needsNewPatrolPoint = false;
            patrolTimer = 0f;
        }
        
        // Move to patrol target
        agent.SetDestination(currentPatrolTarget);
        
        // Apply speed modifiers - blueberry gets additional slowdown
        float finalSpeed = speed * movementSpeedMultiplier * 0.6f; // Slower when patrolling
        if (IsBlueberry())
        {
            finalSpeed *= blueberrySpeedModifier;
        }
        agent.speed = finalSpeed;
        enemyAnimator.SetBool("IsMoving", true);
        
        // Random shooting during movement (1/5 chance every 2 seconds)
        randomShootTimer += Time.fixedDeltaTime;
        if (randomShootTimer >= randomShootInterval)
        {
            randomShootTimer = 0f; // Reset timer
            
            if (canAttack && !attackLocked)
            {
                // Check if we should randomly shoot during movement (regardless of player range)
                if (UnityEngine.Random.Range(0f, 1f) < 0.2f) // 20% chance (1/5)
                {
                    // Random shooting during movement
                    currentState = AttackState.Attacking;
                    canAttack = true;
                    attackLocked = false;
                    hasLanded = true;
                    landingTimer = 0f;
                    
                    // Immediately start the attack
                    StartAttack();
                    return; // Exit to let attack state handle it
                }
            }
        }
        
        // Check if we've reached the patrol target
        float distanceToTarget = Vector2.Distance(transform.position, currentPatrolTarget);
        if (distanceToTarget <= stopThreshold)
        {
            // Stop and wait at patrol point
            StopMovement();
            patrolTimer += Time.fixedDeltaTime;
            
            // Change patrol point after spending enough time here
            if (patrolTimer >= patrolPointChangeTime)
            {
                needsNewPatrolPoint = true;
            }
        }
    }

    private void StopMovement()
    {
        // Completely stop all movement
        agent.speed = 0;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
        
        // Stop the flying animation
        enemyAnimator.SetBool("IsMoving", false);
        
        // Stop any rigidbody movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        // Mark that we've landed
        hasLanded = true;
        landingTimer = 0f;
    }

    private void ApproachPlayer()
    {
        if (targetPlayer == null) return;
        
        Vector3 playerPos = targetPlayer.position;
        Vector3 currentPos = transform.position;
        
        // Check if we're already well-aligned for projectile attack
        if (IsWellAlignedForProjectile(playerPos, currentPos))
        {
            // We're well-aligned, just move closer to attack range
            Vector3 directionToPlayer = (playerPos - currentPos).normalized;
            Vector3 targetPosition = playerPos - (directionToPlayer * attackRange * 0.8f);
            
            agent.SetDestination(targetPosition);
        }
        else
        {
            // Not well-aligned, move to a position that aligns projectiles with player
            Vector3 targetPosition = GetProjectileAlignmentPosition(playerPos, currentPos, projectileAlignmentRadius);
            
            // Check if the aligned position is reachable, if not use fallback
            if (!IsPositionReachable(targetPosition))
            {
                targetPosition = GetFallbackPosition(playerPos, projectileAlignmentRadius);
            }
            
            agent.SetDestination(targetPosition);
        }
        
        // Apply speed modifiers - blueberry gets additional slowdown
        float finalSpeed = speed * movementSpeedMultiplier;
        if (IsBlueberry())
        {
            finalSpeed *= blueberrySpeedModifier;
        }
        agent.speed = finalSpeed;
        enemyAnimator.SetBool("IsMoving", true);
        
        // Check if we're close enough to attack and stop movement
        float distanceToTarget = Vector2.Distance(transform.position, agent.destination);
        if (distanceToTarget <= stopThreshold)
        {
            StopMovement();
        }
    }

    private void RepositionStrategically()
    {
        if (targetPlayer == null) return;
        
        if (needsNewStrategicPosition)
        {
            Vector3 playerPos = targetPlayer.position;
            Vector3 currentPos = transform.position;
            
            // Find a new position that's well-aligned for projectile attacks
            // Try to get a different angle than our current one
            Vector3 currentDirection = (currentPos - playerPos).normalized;
            float currentAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            
            // Find an alignment angle that's different from our current one
            float[] alignmentAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
            float bestAngle = alignmentAngles[0];
            float largestDifference = 0f;
            
            foreach (float angle in alignmentAngles)
            {
                float difference = Mathf.Abs(currentAngle - angle);
                if (difference > 180f) difference = 360f - difference; // Handle angle wrapping
                if (difference > largestDifference)
                {
                    largestDifference = difference;
                    bestAngle = angle;
                }
            }
            
            // Calculate new position at the best angle
            float bestAngleRad = bestAngle * Mathf.Deg2Rad;
            Vector3 newDirection = new Vector3(Mathf.Cos(bestAngleRad), Mathf.Sin(bestAngleRad), 0f);
            strategicTargetPosition = playerPos + (newDirection * projectileAlignmentRadius);
            
            // Check if the new position is reachable, if not use fallback
            if (!IsPositionReachable(strategicTargetPosition))
            {
                strategicTargetPosition = GetFallbackPosition(playerPos, projectileAlignmentRadius);
            }
            
            needsNewStrategicPosition = false;
        }
        
        agent.SetDestination(strategicTargetPosition);
        // Apply speed modifiers - blueberry gets additional slowdown
        float finalSpeed = speed * movementSpeedMultiplier * 0.7f; // Slower when repositioning
        if (IsBlueberry())
        {
            finalSpeed *= blueberrySpeedModifier;
        }
        agent.speed = finalSpeed;
        enemyAnimator.SetBool("IsMoving", true);
        
        // Check if we've reached the reposition target
        float distanceToTarget = Vector2.Distance(transform.position, strategicTargetPosition);
        if (distanceToTarget <= stopThreshold)
        {
            StopMovement();
        }
    }

    private void ReturnToPlayer()
    {
        if (targetPlayer == null) return;
        
        Vector3 playerPos = targetPlayer.position;
        Vector3 currentPos = transform.position;
        
        // Check if we're already well-aligned for projectile attack
        if (IsWellAlignedForProjectile(playerPos, currentPos))
        {
            // We're well-aligned, just move closer to attack range
            Vector3 directionToPlayer = (playerPos - currentPos).normalized;
            Vector3 targetPosition = playerPos - (directionToPlayer * attackRange * 0.8f);
            
            agent.SetDestination(targetPosition);
        }
        else
        {
            // Not well-aligned, move to a position that aligns projectiles with player
            Vector3 targetPosition = GetProjectileAlignmentPosition(playerPos, currentPos, projectileAlignmentRadius * 0.8f);
            
            // Check if the aligned position is reachable, if not use fallback
            if (!IsPositionReachable(targetPosition))
            {
                targetPosition = GetFallbackPosition(playerPos, projectileAlignmentRadius * 0.8f);
            }
            
            agent.SetDestination(targetPosition);
        }
        
        // Apply speed modifiers - blueberry gets additional slowdown
        float finalSpeed = speed * movementSpeedMultiplier;
        if (IsBlueberry())
        {
            finalSpeed *= blueberrySpeedModifier;
        }
        agent.speed = finalSpeed;
        enemyAnimator.SetBool("IsMoving", true);
        
        // Check if we're close enough to attack and stop movement
        float distanceToTarget = Vector2.Distance(transform.position, agent.destination);
        if (distanceToTarget <= stopThreshold)
        {
            StopMovement();
        }
    }

    //-----------------------------------CHASE--------------------------------------------------------------------------------------------------------------------------------------

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
        enemyAnimator.SetBool("IsMoving", true);
        // Apply speed modifiers - blueberry gets additional slowdown
        float finalSpeed = speed * movementSpeedMultiplier;
        if (IsBlueberry())
        {
            finalSpeed *= blueberrySpeedModifier;
        }
        agent.speed = finalSpeed;
    }

    //---------------------------------ATTACK----------------------------------------------------------------------------------------------------------------------------------------

    void StartAttack()
    {
        Debug.Log($"Enemy {gameObject.name}: StartAttack() called - CanAttack: {canAttack}, AttacksPerformed: {attacksPerformed}, MaxAttacks: {maxAttacksPerEngagement}");
        
        if (canAttack && attacksPerformed < maxAttacksPerEngagement)
        {
            Debug.Log($"Enemy {gameObject.name}: Starting attack #{attacksPerformed + 1}");
            
            // Completely stop all movement before attacking
            StopMovement();
            
            // Set attack animation
            Debug.Log($"Enemy {gameObject.name}: About to set IsAttacking to true. Animator null? {enemyAnimator == null}");
            enemyAnimator.SetBool("IsAttacking", true);
            Debug.Log($"Enemy {gameObject.name}: Set IsAttacking to true");
            
            canAttack = false;
            attackLocked = true;
            
            // Increment attack counter
            attacksPerformed++;
            
            // Record when attack started
            attackStartTime = Time.time;

            //ATTACK LOGIC WILL BE CALLED IN ANIMATION EVENT IN SEPERATE SCRIPT
            //enemyAttackScript.Attack(); LIKE THIS BUT BY ANIMATION EVENT
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name}: Cannot attack - CanAttack: {canAttack}, AttacksPerformed: {attacksPerformed}, MaxAttacks: {maxAttacksPerEngagement}");
        }
    }

    public void attackFinish()
    {
        // Check if enough time has passed since attack started
        float timeSinceAttackStarted = Time.time - attackStartTime;
        if (timeSinceAttackStarted < minimumAttackDuration)
        {
            Debug.Log($"Enemy {gameObject.name}: attackFinish() called too early ({timeSinceAttackStarted:F2}s), ignoring. Need at least {minimumAttackDuration}s");
            return; // Don't finish attack yet
        }
        
        Debug.Log($"Enemy {gameObject.name}: attackFinish() called - resetting attack state after {timeSinceAttackStarted:F2}s");
        enemyAnimator.SetBool("IsAttacking", false);
        canAttack = true;
        attackLocked = false;
        
        // Reset landing state for next movement
        hasLanded = false;
        landingTimer = 0f;
        
        // Always return to patrolling after attack
        if (usePatrolMode)
        {
            currentState = AttackState.Patrolling;
            needsNewPatrolPoint = true;
            
            // Reset attack count for next engagement
            if (attacksPerformed >= maxAttacksPerEngagement)
            {
                attacksPerformed = 0;
            }
            
            // Immediately move to next node after attacking (no waiting)
            if (useNodeBasedPatrol && patrolNodes != null && patrolNodes.Length > 0)
            {
                currentNode = GetNextPatrolNode();
                isMovingToNextNode = true;
                needsTacticalPosition = true;
                randomShootTimer = 0f; // Reset random shoot timer for new movement
                Debug.Log($"Enemy {gameObject.name}: Immediately moving to next node after attack");
            }
        }
        else
        {
            // Fallback for non-patrol mode (shouldn't happen with your setup)
            currentState = AttackState.Repositioning;
            stateTimer = 0f;
            needsNewStrategicPosition = true;
            attacksPerformed = 0;
        }
    }
    
    // New method to prepare movement before attack finishes
    public void PrepareForMovement()
    {
        // Set IsMoving to true before the attack animation finishes
        // This ensures the enemy is ready to move immediately after shooting
        enemyAnimator.SetBool("IsMoving", true);
        Debug.Log($"Enemy {gameObject.name}: Preparing for movement - IsMoving set to true");
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

