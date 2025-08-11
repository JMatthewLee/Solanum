using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplotchGenerator : MonoBehaviour
{
    [SerializeField] private GameObject splotch;
    [SerializeField] private Transform splotchTransform;

    public void SplotchInstantiate()
    {
        GameObject newTomSplotch = Instantiate(splotch, splotchTransform.position, Quaternion.identity);
    }
}
