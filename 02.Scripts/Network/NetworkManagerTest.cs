using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace FireEscape.Network
{
    public class NetworkManagerTest : MonoBehaviour
    {
        public TMP_InputField SendText;
        public TextMeshProUGUI ReceiveText;

        public void OnSendButtonClick()
        {
            if (SendText.text != "")
            {
                NetworkManager.Instance.SendAIResponce(SendText.text, "[]");
            }
        }

        public void OnReceiveWebsocketMessage(string message)
        {
            ReceiveText.text = message;
            Debug.Log("ReceiveText.text: " + ReceiveText.text);
        }
    }
}