using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    private bool showAttackPatternSettings = true;
    private bool showBasicSettings = true;
    
    public override void OnInspectorGUI()
    {
        EnemyController controller = (EnemyController)target;
        
        EditorGUILayout.Space();
        
        // Basic Settings Section
        showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "Basic Settings", true);
        if (showBasicSettings)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty enemyMaxHp = serializedObject.FindProperty("enemyMaxHp");
            SerializedProperty enemyCurrentHp = serializedObject.FindProperty("enemyCurrentHp");
            SerializedProperty speed = serializedObject.FindProperty("speed");
            SerializedProperty collisionDammage = serializedObject.FindProperty("collisionDammage");
            
            EditorGUILayout.PropertyField(enemyMaxHp, new GUIContent("Max HP", "Maximum health points for the enemy"));
            EditorGUILayout.PropertyField(enemyCurrentHp, new GUIContent("Current HP", "Current health points"));
            EditorGUILayout.PropertyField(speed, new GUIContent("Movement Speed", "How fast the enemy moves"));
            EditorGUILayout.PropertyField(collisionDammage, new GUIContent("Collision Damage", "Damage dealt on collision with player"));
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Attack Pattern Settings Section
        showAttackPatternSettings = EditorGUILayout.Foldout(showAttackPatternSettings, "Attack Pattern Settings", true);
        if (showAttackPatternSettings)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty attackRange = serializedObject.FindProperty("attackRange");
            SerializedProperty attackDelay = serializedObject.FindProperty("attackDelay");
            SerializedProperty chaseRange = serializedObject.FindProperty("chaseRange");
            SerializedProperty attackContinuationRange = serializedObject.FindProperty("attackContinuationRange");
            SerializedProperty repositionRange = serializedObject.FindProperty("repositionRange");
            SerializedProperty maxAttackDistance = serializedObject.FindProperty("maxAttackDistance");
            SerializedProperty maxAttacksPerEngagement = serializedObject.FindProperty("maxAttacksPerEngagement");
            SerializedProperty repositionTime = serializedObject.FindProperty("repositionTime");
            SerializedProperty strategicPositioningRadius = serializedObject.FindProperty("strategicPositioningRadius");
            SerializedProperty movementSpeedMultiplier = serializedObject.FindProperty("movementSpeedMultiplier");
            SerializedProperty stopThreshold = serializedObject.FindProperty("stopThreshold");
            SerializedProperty immediateAttackRange = serializedObject.FindProperty("immediateAttackRange");
            SerializedProperty landingCommitTime = serializedObject.FindProperty("landingCommitTime");
            SerializedProperty blueberrySpeedModifier = serializedObject.FindProperty("blueberrySpeedModifier");
            SerializedProperty isBlueberryEnemy = serializedObject.FindProperty("isBlueberryEnemy");
            SerializedProperty usePatrolMode = serializedObject.FindProperty("usePatrolMode");
            SerializedProperty useNodeBasedPatrol = serializedObject.FindProperty("useNodeBasedPatrol");
            SerializedProperty patrolNodes = serializedObject.FindProperty("patrolNodes");
            SerializedProperty nodeReachThreshold = serializedObject.FindProperty("nodeReachThreshold");
            SerializedProperty nodeWaitTime = serializedObject.FindProperty("nodeWaitTime");
            SerializedProperty maxDistanceFromNodes = serializedObject.FindProperty("maxDistanceFromNodes");
            SerializedProperty tacticalNodeRadius = serializedObject.FindProperty("tacticalNodeRadius");
            SerializedProperty projectileAlignmentRadius = serializedObject.FindProperty("projectileAlignmentRadius");
            SerializedProperty alignmentTolerance = serializedObject.FindProperty("alignmentTolerance");
            
            EditorGUILayout.PropertyField(attackRange, new GUIContent("Attack Range", "Distance at which enemy will initiate first attack"));
            EditorGUILayout.PropertyField(attackDelay, new GUIContent("Attack Delay", "Delay between attacks"));
            EditorGUILayout.PropertyField(chaseRange, new GUIContent("Chase Range", "Range at which enemy starts chasing player"));
            EditorGUILayout.PropertyField(attackContinuationRange, new GUIContent("Attack Continuation Range", "Range to continue attacking (larger than attack range)"));
            EditorGUILayout.PropertyField(repositionRange, new GUIContent("Reposition Range", "Range where enemy will reposition instead of attacking"));
            EditorGUILayout.PropertyField(maxAttackDistance, new GUIContent("Max Attack Distance", "Maximum distance before enemy gives up and moves"));
            EditorGUILayout.PropertyField(maxAttacksPerEngagement, new GUIContent("Max Attacks Per Engagement", "Maximum number of attacks before repositioning"));
            EditorGUILayout.PropertyField(repositionTime, new GUIContent("Reposition Time", "Time to spend repositioning"));
            EditorGUILayout.PropertyField(strategicPositioningRadius, new GUIContent("Strategic Positioning Radius", "Radius for strategic positioning around player"));
            EditorGUILayout.PropertyField(movementSpeedMultiplier, new GUIContent("Movement Speed Multiplier", "Multiplier for faster enemy movement"));
            EditorGUILayout.PropertyField(stopThreshold, new GUIContent("Stop Threshold", "Distance threshold to consider enemy stopped"));
            EditorGUILayout.PropertyField(immediateAttackRange, new GUIContent("Immediate Attack Range", "Very close range for immediate attack response"));
            EditorGUILayout.PropertyField(landingCommitTime, new GUIContent("Landing Commit Time", "Time to commit to attack after landing"));
            EditorGUILayout.PropertyField(blueberrySpeedModifier, new GUIContent("Blueberry Speed Modifier", "Additional speed reduction for blueberry enemies (0.8 = 80% speed)"));
            EditorGUILayout.PropertyField(isBlueberryEnemy, new GUIContent("Is Blueberry Enemy", "Check this box if this enemy should use blueberry speed modifier"));
            EditorGUILayout.PropertyField(usePatrolMode, new GUIContent("Use Patrol Mode", "Enable independent patrol movement around the map"));
            EditorGUILayout.PropertyField(useNodeBasedPatrol, new GUIContent("Use Node-Based Patrol", "Use predefined patrol nodes instead of random movement"));
            EditorGUILayout.PropertyField(patrolNodes, new GUIContent("Patrol Nodes", "Array of Transform objects defining patrol path"));
            EditorGUILayout.PropertyField(nodeReachThreshold, new GUIContent("Node Reach Threshold", "Distance to consider node reached"));
            EditorGUILayout.PropertyField(nodeWaitTime, new GUIContent("Node Wait Time", "Time to wait at each node before moving to next"));
            EditorGUILayout.PropertyField(maxDistanceFromNodes, new GUIContent("Max Distance From Nodes", "Maximum distance enemy can stray from nodes"));
            EditorGUILayout.PropertyField(tacticalNodeRadius, new GUIContent("Tactical Node Radius", "Radius around nodes for tactical positioning when player spotted"));
            EditorGUILayout.PropertyField(projectileAlignmentRadius, new GUIContent("Projectile Alignment Radius", "Radius for positioning to align projectiles with player"));
            EditorGUILayout.PropertyField(alignmentTolerance, new GUIContent("Alignment Tolerance", "Degrees tolerance for projectile alignment (15 = ±15° from perfect alignment)"));
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // References Section
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        SerializedProperty enemyStateCommunicator = serializedObject.FindProperty("enemyStateCommunicator");
        SerializedProperty dammageFlash = serializedObject.FindProperty("dammageFlash");
        SerializedProperty enemyAnimator = serializedObject.FindProperty("enemyAnimator");
        SerializedProperty playerValueHp = serializedObject.FindProperty("playerValueHp");
        
        EditorGUILayout.PropertyField(enemyStateCommunicator, new GUIContent("Enemy State Communicator", "Reference to the player detection script"));
        EditorGUILayout.PropertyField(dammageFlash, new GUIContent("Damage Flash", "Reference to the damage flash effect"));
        EditorGUILayout.PropertyField(enemyAnimator, new GUIContent("Enemy Animator", "Reference to the enemy's animator component"));
        EditorGUILayout.PropertyField(playerValueHp, new GUIContent("Player Values", "Reference to the player's health component"));
        
        EditorGUI.indentLevel--;
        
        // Apply changes
        serializedObject.ApplyModifiedProperties();
        
        // Add some helpful information
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Attack Pattern Behavior:\n" +
            "• Enemy follows predefined patrol nodes in sequence\n" +
            "• Stays near nodes when player is spotted for tactical advantage\n" +
            "• Attacks up to 2 times per engagement\n" +
            "• Returns to patrol after max attacks or if too far\n" +
            "• Uses projectile alignment for accurate attacks", MessageType.Info);
    }
}
