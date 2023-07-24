using UnityEngine;
using Mirror;

public class ServerHostUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.SetActive(NetworkServer.active && NetworkClient.isConnected);
    }
}
