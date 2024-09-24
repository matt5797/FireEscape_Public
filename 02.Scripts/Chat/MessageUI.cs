using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FireEscape.Chat
{
    public class MessageUI : MonoBehaviour
    {
        public TMP_Text messageText;

        public void Initialize(Message message)
        {
            messageText.text = message.message;
        }
    }
}