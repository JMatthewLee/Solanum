using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool Parent;

    public virtual void OnDisable() //VIRTUAL VOID METHOD NECESSARY FOR POOLED OBJECTS
    {
        Parent.ReturnObjectToPool(this);
    }
}