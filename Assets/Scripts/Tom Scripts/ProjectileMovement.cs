using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileMovement : AutoDestroyPoolableObject
{
    public GameObject breakParticlesPrefab;
    private Rigidbody2D rb;

    [SerializeField] private float force;
    [SerializeField] private float projectileInstantDamage;

    //[SerializeField] private float totalDOT; 
    //[SerializeField] private float tickSpeedDOT;
    //[SerializeField] private float tickCountsDOT;
    //private float dmgPerTick= totalDOT/tickCountsDOT;  //Variables used in future Damage Over Time Coroutines

    private Vector3 targetDirection;

    public float Force => force; //Inherited from Projectile Rotation

    public override void OnEnable() //On enabling the Pooled Object
    {
        base.OnEnable(); //Called in Super Class

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.velocity = new Vector2(targetDirection.x, targetDirection.y).normalized * force;
    }

    public void Initialize(Vector3 mousePosition, Vector3 direction) //Called in Projectile Rotation Used to Initialize MousePositon and Direction To Find the Velocity and Rotation
    {
        Vector3 targetDirection = direction;

        Vector3 rotation = mousePosition - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotz - 45); // Adjust based on your the desired sprite orientation

        rb.velocity = new Vector2(targetDirection.x, targetDirection.y).normalized * force; //ESSENTIAL TO SET VELCOITY LIKE THIS SINCE 2D HAS NO Z DIRECTION

        Debug.Log("New:" + targetDirection);
    }

    public override void OnDisable() //Disabling the Projectile and Returning it to a Reusable State
    {
        base.OnDisable(); //Called in the SuperClass

        rb.velocity = Vector2.zero;     
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision) //BASIC LOGIC FOR COMMUNICATING DAMAGE TO ENEMIES OR DIABLING THE PROJECTILE
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Grid Solid"))
        {
            GameObject newPickParticles = Instantiate(breakParticlesPrefab, transform.position, Quaternion.identity);

            if (collision.isTrigger && collision.gameObject.TryGetComponent<EnemyValues>(out EnemyValues enemyValuesComponent))
            {
                enemyValuesComponent.TakeDamage(projectileInstantDamage);
            }
            else if (collision.isTrigger && collision.gameObject.TryGetComponent<EnemyController>(out EnemyController enemyControllerComponent))
            {
                enemyControllerComponent.TakeDamage(projectileInstantDamage);
            }

            Disable(); // "Destroy" the projectile
        }
    }
}