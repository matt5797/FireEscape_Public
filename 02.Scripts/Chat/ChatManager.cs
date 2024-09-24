using UnityEngine;
using System.Collections.Generic;
using System;
using YamlDotNet.Serialization;
using FireEscape.Action;
using FireEscape.Network;
using FireEscape.Ability;
using MoreMountains.TopDownEngine;

namespace FireEscape.Chat
{
    [System.Serializable]
    public class Message
    {
        public enum Sender { Player, Child, System };
        public Sender sender;
        public string message;

        public Message(Sender sender, string message)
        {
            this.sender = sender;
            this.message = message;
        }
    }
    
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance { get; private set; }

        private CharacterSpeak _characterSpeak;

        public List<MessageGroup> messageGroups = new List<MessageGroup>();

        public List<Message> messagesLog = new List<Message>();
        public List<Message> inputMessages = new List<Message>();

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
#else
                Destroy(gameObject);
#endif
            }

            messageGroups.Clear();
            messagesLog.Clear();
            inputMessages.Clear();
        }

        private void Update()
        {
            if (_characterSpeak==null)
            {
                _characterSpeak = LevelManager.Instance.Players[0].GetComponent<Character>().FindAbility<CharacterSpeak>();
            }
        }

        public void OnPlayerSendDone()
        {
            PossibleActionList possibleActionList = new PossibleActionList(AIActionManager.Instance.GetAvailableActions());
            string functions = JsonUtility.ToJson(possibleActionList);

            // Create a PlayerMessageGroup
            PlayerMessageGroup playerMessageGroup = new PlayerMessageGroup();
            // Populate playerMessageGroup data
            playerMessageGroup.CurrentState = ""; // TODO: Populate this
            playerMessageGroup.ChildEmotion = new ChildEmotion(2); // TODO: Populate this
            playerMessageGroup.SurroundingObjects = new List<SurroundingObject>(); // TODO: Populate this
            // playerMessageGroup.PossibleActions = new List<PossibleAction>(); // TODO: Populate this
            playerMessageGroup.Messages = new List<string>();
            // Add all input messages to playerMessageGroup
            foreach (Message message in inputMessages)
            {
                messagesLog.Add(message);
                playerMessageGroup.Messages.Add(message.message);
            }

            // Serialize to YAML
            string yamlData = playerMessageGroup.SerializeToYAML();

            // For now, let's just print it to console
            Debug.Log(yamlData);
            Debug.Log(functions);

            // Send the data to server (this should be handled by NetworkManager)
            NetworkManager.Instance.SendAIResponce(yamlData, functions);

            // Add to message groups
            messageGroups.Add(playerMessageGroup);

            inputMessages.Clear();
        }

        public void OnChildMessageReceived(string childMessage)
        {
            ChildMessageGroup childMessageGroup = new ChildMessageGroup();
            childMessageGroup.Messages = new List<string>() { childMessage };

            // Add to message groups
            messageGroups.Add(childMessageGroup);

            // Add to messages log
            messagesLog.Add(new Message(Message.Sender.Child, childMessage));

            // Update UI
            ChatUI.Instance.AddChildMessageToChat(childMessage);
            _characterSpeak.Speak(childMessage);
        }
    }
}
