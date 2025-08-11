using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.ParticleSystem;

public class BerryProjectilesScript : MonoBehaviour
{
    public GameObject breakParticlesPrefab;
    private Rigidbody2D rb;

    [SerializeField] private float force;
    [SerializeField] private int projectileInstantDamage;

    private float xDirection;
    private float yDirection;
    private float xRotation;
    private float yRotation;

    //[SerializeField] private float totalDOT; 
    //[SerializeField] private float tickSpeedDOT;
    //[SerializeField] private float tickCountsDOT;
    //private float dmgPerTick= totalDOT/tickCountsDOT;  //Variables used in future Damage Over Time Coroutines

    public void setDirection(float xDirValue, float yDirValue, float xRotValue, float yRotValue)
    {
        xDirection = xDirValue;
        yDirection = yDirValue;
        xRotation = xRotValue;
        yRotation = yRotValue;
    }

    private void Start()
    {
        Debug.Log("Script Started");

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(xDirection, yDirection).normalized * force;

        //rotate the projectile
        float rotz = Mathf.Atan2(yRotation, xRotation) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotz + 0); //Might need to add 135, 90 or 45 to rotation for proper orientation

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is in the "Destroyable" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Grid Solid"))
        {

            //Instantiating the Particles onHit, ensure PLAY ON AWAKE IS CHECKED
            GameObject newPickParticles = Instantiate(breakParticlesPrefab, transform.position, Quaternion.identity); //maybe use transform. position
            //newPickParticles.GetComponent<Material>(); // Maybe use this to set the colour of the particles later


            Destroy(gameObject); // destroy the projectile

            if (collision.isTrigger && collision.gameObject.TryGetComponent < PlayerValues>(out PlayerValues playerHealthComponent))
            {
                playerHealthComponent.RecieveDamage(projectileInstantDamage);

                //enemyValuesComponent.TakeDOT(tickSpeedDOT, tickCounts, dmgPerTick);  
                //POSSIBLE FUNCTION IF THE DAMAGE IS DONE AS DMG OVER TIME
            }
        }
    }

}