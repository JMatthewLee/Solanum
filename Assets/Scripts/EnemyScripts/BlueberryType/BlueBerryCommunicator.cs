using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBerryCommunicator : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyValues enemyValues;
    [SerializeField] private PlayerValues playerValueHp;

    [SerializeField] private int explosionDamage;

    private bool hasExploded = false;

    private Collider2D ExplosionCollider;

    private void Start()
    {
        ExplosionCollider = GetComponent<CapsuleCollider2D>();
        playerValueHp = GameObject.Find("Player").GetComponent<PlayerValues>();
    }

    public void WakeUpFinish()
    {
        enemyAI.WakeUp();
    }

    public void Death()
    {
        enemyValues.Death(); //Check this out maybe add more
    }

    public void Explode()
    {
        ExplosionCollider.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !hasExploded)
        {
            Debug.Log("Blueberrycollide");
            playerValueHp.RecieveDamage(explosionDamage);
            hasExploded = true;
        }
    }
}
