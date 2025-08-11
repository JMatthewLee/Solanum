using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ProjectileRotation : MonoBehaviour
{

    [SerializeField] private ProjectileMovement projectile;
    [SerializeField] private Transform projectileTransform;
    [SerializeField] bool canFire;
    [SerializeField] private float timeBetweenFiring;

    private ObjectPool ProjectilePool;

    private float timer;
    private Camera mainCam;
    public Vector3 mousePosition { get; private set; }

    private void Awake()
    {
        ProjectilePool = ObjectPool.CreateInstance(projectile, 100);
    }

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void RotateProjectile(Vector2 cursorPosition)
    {
        //setting rotation and position of the parent
        mousePosition = mainCam.ScreenToWorldPoint(cursorPosition);
        Vector3 rotation = mousePosition - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotz);
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring) 
            {
                canFire = true;
                timer   = 0;
            }
        }

    }

    public void InstantiateProjectile() //I DONT EVEN WANT TO START WITH THIS ONE GOOD LUCK
    {
        if (canFire)
        {
            canFire = false;

            PoolableObject instance = ProjectilePool.GetObject(); //Grab the next Pooled Object

            if (instance != null)
            {
                instance.transform.SetParent(null, false); //DO NOT USE PARENTS TRANSFORM
                instance.transform.position = projectileTransform.position;// Instead Use the Desired Transform Object Transform

                Vector3 direction = (mousePosition - projectileTransform.position).normalized;

                instance.gameObject.SetActive(true); //Activate the Instance

                instance.GetComponent<ProjectileMovement>().Initialize(mousePosition, direction); //Communicate The Values
            }
            else
            {
                Debug.LogError("Failed to get object from pool.");
            }
        }
    }

}
