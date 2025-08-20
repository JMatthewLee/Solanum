using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    [Header("Movement Configuration")]
    [SerializeField] protected EnemyMovementConfig movementConfig;
    
    protected NavMeshAgent agent;
    protected Transform targetPlayer;
    protected float lastTargetUpdateTime;
    
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupNavMeshAgent();
    }
    
    protected virtual void SetupNavMeshAgent()
    {
        if (agent == null || movementConfig == null) return;
        
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        // Apply configuration settings
        agent.radius = movementConfig.AgentRadius;
        agent.avoidancePriority = movementConfig.GetRandomAvoidancePriority();
        agent.obstacleAvoidanceType = movementConfig.ObstacleAvoidanceType;
        agent.autoBraking = movementConfig.AutoBraking;
        agent.stoppingDistance = movementConfig.StoppingDistance;
    }
    
    protected virtual void ChasePlayer()
    {
        if (targetPlayer == null || agent == null) return;
        
        // Update target position periodically to create more dynamic movement
        if (Time.time - lastTargetUpdateTime > movementConfig.TargetUpdateInterval)
        {
            Vector3 targetPosition = targetPlayer.position + Random.insideUnitSphere * movementConfig.TargetOffsetRadius;
            agent.SetDestination(targetPosition);
            lastTargetUpdateTime = Time.time;
        }
    }
    
    protected virtual void SetTargetPlayer(Transform player)
    {
        targetPlayer = player;
    }
    
    // New method for strategic positioning that considers the enemy's attack pattern
    protected virtual Vector3 GetStrategicPosition(Vector3 playerPosition, float radius, Vector3 currentPosition)
    {
        // Calculate direction from player to current enemy position
        Vector3 directionFromPlayer = (currentPosition - playerPosition).normalized;
        
        // Get perpendicular directions for strategic positioning
        Vector3 perpendicular1 = new Vector3(-directionFromPlayer.y, directionFromPlayer.x, 0);
        Vector3 perpendicular2 = new Vector3(directionFromPlayer.y, -directionFromPlayer.x, 0);
        
        // Choose one of the perpendicular directions randomly
        Vector3 strategicDirection = UnityEngine.Random.value > 0.5f ? perpendicular1 : perpendicular2;
        
        // Add some randomness to the strategic position
        float randomAngle = UnityEngine.Random.Range(-30f, 30f);
        Vector3 rotatedDirection = Quaternion.Euler(0, 0, randomAngle) * strategicDirection;
        
        return playerPosition + rotatedDirection * radius;
    }
    
    // Method to check if a position is reachable
    protected virtual bool IsPositionReachable(Vector3 targetPosition)
    {
        if (agent == null) return false;
        
        NavMeshPath path = new NavMeshPath();
        return agent.CalculatePath(targetPosition, path);
    }
    
    // Method to get a fallback position if the strategic position is unreachable
    protected virtual Vector3 GetFallbackPosition(Vector3 playerPosition, float radius)
    {
        // Try different angles until we find a reachable position
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 testPosition = playerPosition + direction * radius;
            
            if (IsPositionReachable(testPosition))
            {
                return testPosition;
            }
        }
        
        // If all fail, return a simple offset position
        return playerPosition + Vector3.right * radius;
    }
}
