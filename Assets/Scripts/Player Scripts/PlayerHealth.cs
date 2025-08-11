using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerHealth : MonoBehaviour
{
    static public int maxHp = 10;
    static public int hp;

    void Start()
    {
        hp = maxHp;
    }


}
