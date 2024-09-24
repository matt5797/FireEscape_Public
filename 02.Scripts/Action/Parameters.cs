using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace FireEscape.Action
{
    [System.Serializable]
    public class Parameters
    {
        public string type;
        public Properties properties;
        [SerializeField]
        public List<string> required;

        public Parameters()
        {
            type = "object";
            properties = new Properties
            {
                childMessage = new Message(),
            };
            required = new List<string>() { "childMessage", "isMessageAfterAction" };
        }
    }

    [System.Serializable]
    public class Properties
    {
        public Message childMessage;
        public IsMessageAfterAction isMessageAfterAction;
    }

    [System.Serializable]
    public class Message
    {
        public string type = "string";
        public string description = "This represents the statements made by the child character prior to, in the course of, and subsequent to the action. e.g. Okay, I'll open the door";
    }

    [System.Serializable]
    public class IsMessageAfterAction
    {
        public string type = "boolean";
        public string description = "If true, the message is said after the action, e.g. the door is open";
    }
}