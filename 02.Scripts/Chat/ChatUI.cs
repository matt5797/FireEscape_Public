using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FireEscape.Action;

namespace FireEscape.Chat
{
    public class ChatUI : MonoBehaviour
    {
        public static ChatUI Instance { get; private set; }

        public TMP_InputField chatInput;
        public Button sendButton;
        public Button doneButton;
        public RectTransform messageContent;

        public GameObject playerMessagePrefab;
        public GameObject childMessagePrefab;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            sendButton.onClick.AddListener(OnSendButtonPressed);
            doneButton.onClick.AddListener(OnPlayerSendDone);

            //Hide();
        }

        public void OnOff(bool onoff)
        {
            if (onoff)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void AddPlayerMessageToChat(string message)
        {
            Debug.Log("Adding player message to chat: " + message);

            Message newMessage = new Message(Message.Sender.Player, message);
            ChatManager.Instance.inputMessages.Add(newMessage);
            GameObject messageObject = Instantiate(playerMessagePrefab, messageContent);
            messageObject.GetComponent<MessageUI>().Initialize(newMessage);
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageObject.GetComponent<RectTransform>());
        }

        public void AddChildMessageToChat(string message)
        {
            Message newMessage = new Message(Message.Sender.Child, message);
            ChatManager.Instance.messagesLog.Add(newMessage);
            GameObject messageObject = Instantiate(childMessagePrefab, messageContent);
            messageObject.GetComponent<MessageUI>().Initialize(newMessage);
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageObject.GetComponent<RectTransform>());
        }

        public void OnSendButtonPressed()
        {
            if (chatInput.text != "")
            {
                AddPlayerMessageToChat(chatInput.text);
                chatInput.text = "";
            }
        }

        public void OnPlayerSendDone()
        {
            ChatManager.Instance.OnPlayerSendDone();
        }
    }
}
