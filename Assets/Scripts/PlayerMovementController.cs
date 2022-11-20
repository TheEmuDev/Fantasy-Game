using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody2D = null;
    [SerializeField] private Animator animator = null;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private Vector2 previousInput;

    public override void OnStartAuthority()
    {
        enabled = true;

        InputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        InputManager.Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]

    private void Update() => Move();

    [Client]
    private void SetMovement(Vector2 movement) => previousInput = movement;

    [Client]
    private void ResetMovement() => previousInput = Vector2.zero;

    [Client]

    private void Move()
    {
        Vector2 up = rigidBody2D.transform.up;
        Vector2 right = rigidBody2D.transform.right;

        Vector2 movement = right * previousInput.x + up * previousInput.y;

        if (previousInput.y == 0f) {
            if (previousInput.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
            if (previousInput.x > 0f)
                transform.localScale = Vector3.one;
        }

        animator.SetFloat("Horizontal", previousInput.x);
        animator.SetFloat("Vertical", previousInput.y);
        animator.SetFloat("Speed", previousInput.sqrMagnitude);

        rigidBody2D.MovePosition(rigidBody2D.position + movementSpeed * Time.deltaTime * movement);
    }
}
