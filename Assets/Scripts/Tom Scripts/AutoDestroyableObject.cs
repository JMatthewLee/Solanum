using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyPoolableObject : PoolableObject
{
    public float AutoDestroyTime = 12f;

    private const string DisableMethodName = "Disable";

    public virtual void OnEnable() //VIRTUAL VOID METHOD NECESSARY FOR POOLED OBJECTS
    {
        CancelInvoke(DisableMethodName);

        Invoke(DisableMethodName, AutoDestroyTime);
    }

    public virtual void Disable() //VIRTUAL VOID METHOD NECESSARY FOR POOLED OBJECTS
    {
        gameObject.SetActive(false);
    }
}
