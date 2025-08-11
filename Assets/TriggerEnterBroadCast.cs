using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterBroadCast : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //RASPBERRIES AND NEW ENEMIES
            foreach (EnemyController enemyController in EnemyController.allControllerEnemies)
            {
                enemyController.WakeUp();
            }
            //BLUEBERRY
            Debug.Log("working");

            foreach (EnemyValues enemyValues in EnemyValues.allEnemyValues)
            {
                enemyValues.WakeUp();
            }

        }
    }
}
