using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMovementConfig", menuName = "Solanum/Enemy Movement Config")]
public class EnemyMovementConfig : ScriptableObject
{
    [Header("NavMesh Agent Settings")]
    [SerializeField] private float agentRadius = 0.3f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private bool autoBraking = true;
    
    [Header("Avoidance Settings")]
    [SerializeField] private UnityEngine.AI.ObstacleAvoidanceType obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    [SerializeField] private int minAvoidancePriority = 1;
    [SerializeField] private int maxAvoidancePriority = 100;
    
    [Header("Target Offset Settings")]
    [SerializeField] private float targetOffsetRadius = 0.3f;
    [SerializeField] private float targetUpdateInterval = 1.0f;
    
    // Public properties to access the values
    public float AgentRadius => agentRadius;
    public float StoppingDistance => stoppingDistance;
    public bool AutoBraking => autoBraking;
    public UnityEngine.AI.ObstacleAvoidanceType ObstacleAvoidanceType => obstacleAvoidanceType;
    public int MinAvoidancePriority => minAvoidancePriority;
    public int MaxAvoidancePriority => maxAvoidancePriority;
    public float TargetOffsetRadius => targetOffsetRadius;
    public float TargetUpdateInterval => targetUpdateInterval;
    
    // Method to get a random avoidance priority
    public int GetRandomAvoidancePriority()
    {
        return Random.Range(minAvoidancePriority, maxAvoidancePriority + 1);
    }
}
