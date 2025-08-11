using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueAnimator : MonoBehaviour
{
    private Animator animator;

    private float lastMoveX = 0f;
    private float lastMoveY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the child object!");
        }
    }

    public void UpdateMovement(Vector2 moveInput)
    {
        if (animator == null) return;

        animator.SetBool("IsMoving", true);
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (moveInput.x != 0f) // If Moving on X, Assign X Movement & Assign Last Y movement 0
        {
            lastMoveX = moveInput.x;
            lastMoveY = 0f;
        }

        if (moveInput.y != 0f) // If Moving on Y, Assign Y Movement  
        {
            lastMoveY = moveInput.y;

            if (lastMoveX != 1f) // Check Last X Movement; If 1, look right else look left
            {
                animator.SetFloat("InputX", lastMoveX);
            }
            else
            {
                animator.SetFloat("InputX", lastMoveX);
            }
        }
    }

    public void StopMovement()
    {
        if (animator == null) return;

        animator.SetBool("IsMoving", false);
        animator.SetFloat("LastInputX", lastMoveX);
        animator.SetFloat("LastInputY", lastMoveY);
    }
}