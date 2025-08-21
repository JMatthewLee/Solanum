using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class EnemyValues : MonoBehaviour
{

    public static event Action<EnemyValues> EnemyHasDied; //BRODCAST THIS ENEMY HAS DIED

    public static List<EnemyValues> allEnemyValues = new List<EnemyValues>();  //Used to BroadCast Trigger Functions (E.G trigger to wakeup all enemies)

    [SerializeField] private PlayerDetection playerDetection; //reference to the player detection script used to call the sleep function
    [SerializeField] private DammageFlash dammageFlash;


    [SerializeField] private float enemyMaxHp;
    [SerializeField] private float enemyCurrentHp;

    void OnEnable()
    {
        allEnemyValues.Add(this);
    }

    void OnDisable()
    {
        allEnemyValues.Remove(this);
    }

    private void Start()
    {
        enemyCurrentHp = enemyMaxHp;
    }

    public void TakeDamage(float damagetaken)
    {
        if (playerDetection != null)
        {
            playerDetection.WakeUp();
        }
        
        enemyCurrentHp -= damagetaken;
        if (enemyCurrentHp <= 0)
        {
            dammageFlash.CallHitFlash();
            enemyCurrentHp = 0;
            playerDetection.Sleep(true,true);
        }
        else
        {
            dammageFlash.CallHitFlash();
        }
    }
    
    // REMOVED: WakeUp method - wake state is now managed by EnemyAI

    public void Death()//Called by animation event Destroy the game object once Death animation is finished
    {
        //Maybe ADD A DEATH SFX
        Destroy(gameObject);
        EnemyHasDied?.Invoke(this); //BROADCAST THIS ENEMY HAS DIED
    }


}
