using Mirror;
using UnityEngine;
using TMPro;

namespace QuickStart {
    public class PlayerScript : NetworkBehaviour {
        public TextMesh playerNameText;
        public GameObject floatingInfo;
        private Material playerMaterialClone;

        [SyncVar (hook = nameof (OnNameChanged))]
        public string playerName;

        [SyncVar (hook = nameof (OnColorChanged))]
        public Color playerColor = Color.white;

        private ServerSceneManager sceneScript;

        void OnNameChanged (string _Old, string _New) {
            playerNameText.text = playerName;
        }

        void OnColorChanged (Color _Old, Color _New) {
            playerNameText.color = _New;
            playerMaterialClone = new Material (GetComponent<Renderer> ().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer> ().material = playerMaterialClone;
        }

        void Awake () {
            //allow all players to run this
            sceneScript = GameObject.FindObjectOfType<ServerSceneManager> ();
        }

        [Command]
        public void CmdSendPlayerMessage (string message) {
            if (sceneScript)
                sceneScript.statusText = $"{playerName} says: {message}";
        }

        [Command]
        public void CmdSetupPlayer (string _name, Color _col) {
            //player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;
            sceneScript.statusText = $"{playerName} joined.";
        }

        public override void OnStartLocalPlayer () {
            sceneScript.playerScript = this;
            Camera.main.transform.SetParent (transform);
            Camera.main.transform.localPosition = new Vector3 (0, 0, -10);

            if (!isLocalPlayer) {
                floatingInfo.transform.localScale = new Vector3 (-1, 0, 0);
            }

            string name = "Player" + Random.Range (100, 999);
            Color color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
            CmdSetupPlayer (name, color);
        }

        void Update () {
            float moveX = Input.GetAxis ("Horizontal") * Time.deltaTime * 5.0f;
            float moveY = Input.GetAxis ("Vertical") * Time.deltaTime * 5.0f;

            transform.Translate (moveX, moveY, 0);
        }
    }
}