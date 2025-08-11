using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackberryAttackScript : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileTransform;

    public void Attack()
    {
        GameObject newBerryProjectile1 = Instantiate(projectile, projectileTransform.position, Quaternion.identity);
        newBerryProjectile1.GetComponent<BerryProjectilesScript>().setDirection(1, 1, -1, 1);
        GameObject newBerryProjectile2 = Instantiate(projectile, projectileTransform.position, Quaternion.identity);
        newBerryProjectile2.GetComponent<BerryProjectilesScript>().setDirection(-1, -1, -1, 1);
        GameObject newBerryProjectile3 = Instantiate(projectile, projectileTransform.position, Quaternion.identity);
        newBerryProjectile3.GetComponent<BerryProjectilesScript>().setDirection(-1, 1, 1, 1);
        GameObject newBerryProjectile4 = Instantiate(projectile, projectileTransform.position, Quaternion.identity);
        newBerryProjectile4.GetComponent<BerryProjectilesScript>().setDirection(1, -1, 1, 1);

        Debug.Log("ATTACK");
    }
}



//inRangeLocation = (transform.position).normalized; //CAN BE USED TO FIND PLAYER LAST LOCATION
//lastTargetLocation = (targetPlayer.position).normalized; //CAN BE USED TO FIND PLAYER LAST LOCATION

//StartCoroutine(AttackDelay()); //ATTACK DELAY USED ON OTHER ENEMIES



//CAN BE USED TO FIND PLAYER LAST LOCATION
//[SerializeField] private float postAttackDelay; //USED WHEN ENEMY WILL CONTINUE TO LIVE AFTER ATTACK
//private Vector2 inRangeLocation;
//private Vector2 lastTargetLocation;