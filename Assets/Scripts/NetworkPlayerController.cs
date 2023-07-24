using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.SimpleWeb;
using Telepathy;
using Cinemachine;

public class NetworkPlayerController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody2D = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private Vector2 movementInput;
    private Controls controls;


    private void Awake()
    {
        controls = new();
        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
    }

    public override void OnStartAuthority()
    {
        controls.Player.Enable();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("NetworkPlayerController:OnStartLocalPlayer");
        base.OnStartLocalPlayer();

        StartCoroutine(SetCameraFollowTarget());
    }

    private IEnumerator SetCameraFollowTarget()
    {
        yield return new WaitForSeconds(0.1f);

        CinemachineVirtualCamera cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = transform;
            cinemachineCamera.LookAt = transform;
        }
        else
        {
            Debug.LogError("Couldn't find a Cinemachine Virtual Camera in the scene.");
        }
    }

    [ClientCallback]
    private void OnDisable()
    {
        controls.Player.Disable();
    }  

    [Client]
    private void Update() => Move();

    [Client]
    private void Move()
    {
        if (!isOwned || !NetworkClient.ready) { return; }

        Vector2 movement = movementSpeed * Time.deltaTime * movementInput;

        // Command to tell server about movement
        CmdMove(movement);

        if (movementInput.y == 0f)
        {
            if (movementInput.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
            if (movementInput.x > 0f)
                transform.localScale = Vector3.one;
        }

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.sqrMagnitude);
    }

    [Command]
    private void CmdMove(Vector2 movement)
    {
        rigidBody2D.MovePosition(rigidBody2D.position + movement);
    }
}
