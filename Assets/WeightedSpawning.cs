using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedSpawning : MonoBehaviour
{
    
    enum EnemyState
    {
        Sleeping,
        Idling,
        Roaming,
        Moving,
        Attacking,
        Shuffling
    }

    EnemyState currentState;

    void Start()
    {
        currentState = EnemyState.Sleeping;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
