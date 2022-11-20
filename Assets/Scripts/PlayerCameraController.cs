using Mirror;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;


    public override void OnStartAuthority()
    {
        Debug.Log("OnStartAuthority Player Camera");
        virtualCamera.gameObject.SetActive(true);
        virtualCamera.enabled = true;
    }
}
