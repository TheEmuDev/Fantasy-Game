using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickStart {
    public class ServerSceneManager : NetworkBehaviour {
        public TextMeshProUGUI canvasStatusText;
        public TMP_InputField canvasInputField;

        public PlayerScript playerScript;

        [SyncVar (hook = nameof (OnStatusTextChanged))]
        public string statusText;

        void OnStatusTextChanged (string _Old, string _New) {
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage () {
            if (playerScript != null)
                playerScript.CmdSendPlayerMessage (canvasInputField.text);
        }
    }
}