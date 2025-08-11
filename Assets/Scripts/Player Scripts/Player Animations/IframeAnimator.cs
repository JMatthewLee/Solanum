using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IframeAnimator : MonoBehaviour
{
    public Animator animator;

    public void StartIframe()
    {
        //triggering the animation
        animator.SetTrigger("IsBlocking");
    }
}
