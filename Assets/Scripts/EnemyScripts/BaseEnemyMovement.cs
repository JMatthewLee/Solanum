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
}
