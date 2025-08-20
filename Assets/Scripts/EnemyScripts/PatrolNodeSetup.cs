using UnityEngine;

public class PatrolNodeSetup : MonoBehaviour
{
    [Header("Patrol Node Setup")]
    [SerializeField] private int numberOfNodes = 4;
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private bool createNodes = false;
    [SerializeField] private Transform[] createdNodes;
    
    [Header("Enemy Reference")]
    [SerializeField] private EnemyController enemyController;
    
    void Update()
    {
        if (createNodes)
        {
            CreatePatrolNodes();
            createNodes = false;
        }
    }
    
    void CreatePatrolNodes()
    {
        // Clear existing nodes
        if (createdNodes != null)
        {
            foreach (Transform node in createdNodes)
            {
                if (node != null)
                    DestroyImmediate(node.gameObject);
            }
        }
        
        createdNodes = new Transform[numberOfNodes];
        
        for (int i = 0; i < numberOfNodes; i++)
        {
            // Calculate position around the enemy
            float angle = (360f / numberOfNodes) * i;
            float radians = angle * Mathf.Deg2Rad;
            
            Vector3 nodePosition = transform.position + new Vector3(
                Mathf.Cos(radians) * patrolRadius,
                Mathf.Sin(radians) * patrolRadius,
                0f
            );
            
            // Create the node
            GameObject node = new GameObject($"PatrolNode_{i + 1}");
            node.transform.position = nodePosition;
            node.transform.SetParent(transform);
            
            // Add a visual indicator (optional)
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            {
                node.AddComponent<SpriteRenderer>().sprite = sr.sprite;
                node.GetComponent<SpriteRenderer>().color = Color.yellow;
                node.GetComponent<SpriteRenderer>().sortingOrder = 10;
                node.transform.localScale = Vector3.one * 0.5f;
            }
            
            createdNodes[i] = node.transform;
        }
        
        // Auto-assign to enemy controller if available
        if (enemyController != null)
        {
            // Use reflection to set the patrolNodes field
            var field = typeof(EnemyController).GetField("patrolNodes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(enemyController, createdNodes);
                Debug.Log($"Patrol nodes assigned to {enemyController.name}");
            }
        }
        
        Debug.Log($"Created {numberOfNodes} patrol nodes around {gameObject.name}");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw patrol area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        // Draw node connections
        if (createdNodes != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < createdNodes.Length; i++)
            {
                if (createdNodes[i] != null)
                {
                    Gizmos.DrawWireSphere(createdNodes[i].position, 0.5f);
                    
                    // Draw line to next node
                    int nextIndex = (i + 1) % createdNodes.Length;
                    if (createdNodes[nextIndex] != null)
                    {
                        Gizmos.DrawLine(createdNodes[i].position, createdNodes[nextIndex].position);
                    }
                }
            }
        }
    }
}
