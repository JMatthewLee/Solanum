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

    public void TakeDamage(float damagetaken) //Maybe add A fixed update to check for DOT (Dmg Over Time) IN THE FUTURE FOR NOW LEAVE IT
    {
        WakeUp();
        playerDetection.RestartEnemySleepCountdown();

        enemyCurrentHp -= damagetaken;
        if (enemyCurrentHp <= 0)
        {
            dammageFlash.CallHitFlash();
            enemyCurrentHp = 0;
            playerDetection.Sleep(true,true);
        }
        else
        {
            dammageFlash.CallHitFlash(); //Play some sort of enemy hurt animation (MAYBE JUST ADD A WHITE 30% TO THE CURRENT SPRITE)
            //Play enemy hurt sound (MAYBE LIKE A Squish?)
        }
    }
    public void WakeUp()
    {
        playerDetection.ResetEnemyAwakeStatus();
    }

    public void Death()//Called by animation event Destroy the game object once Death animation is finished
    {
        //Maybe ADD A DEATH SFX
        Destroy(gameObject);
        EnemyHasDied?.Invoke(this); //BROADCAST THIS ENEMY HAS DIED
    }


}
