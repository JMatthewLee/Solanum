using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private Vector2 movementInput, cursorPosition;
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private RogueAnimator rogueAnimator;
    [SerializeField] private IframeAnimator iframeAnimator;
    [SerializeField] private ProjectileRotation projectileRotation;
    [SerializeField] private InputActionReference cursorposition, movement, attack, block;

    private void OnEnable()
    {
        block.action.performed += Block;
        attack.action.performed += Attack;
    }
    private void OnDisable()
    {
        block.action.performed -= Block;
        attack.action.performed -= Attack;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Initialize RigidBody2D
    }

    // Update is called once per frame
    private void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();
        cursorPosition = cursorposition.action.ReadValue<Vector2>();
        projectileRotation.RotateProjectile(cursorPosition);

        rb.velocity = movementInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (rogueAnimator == null) return;

        movementInput = context.ReadValue<Vector2>();

        if (context.canceled) // If keydown is stopped, assign Last Input of X and Y to Idle animator
        {
            rogueAnimator.StopMovement();
        }
        else
        {
            rogueAnimator.UpdateMovement(movementInput);
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        iframeAnimator.StartIframe();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        projectileRotation.InstantiateProjectile();
    }
}