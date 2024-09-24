using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireEscape.Action;

namespace FireEscape.Chat
{
    [System.Serializable]
    public class PossibleActionList
    {
        [SerializeField]
        public List<PossibleAction> possibleActions = new List<PossibleAction>();

        public PossibleActionList(List<AIAction> actionList)
        {
            foreach (AIAction action in actionList)
            {
                possibleActions.Add(new PossibleAction(action));
            }
        }
    }

    [System.Serializable]
    public struct PossibleAction
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string description;
        [SerializeField]
        public Parameters parameters;

        public PossibleAction(AIAction action)
        {
            name = action.name + "___" + action.GetInstanceID();
            description = action.description;
            parameters = action.parameters;
        }
    }
}

/*
{
    "name": "OpenDoor___33586",
    "description": "Open the door",
    "parameters": {
        "type": "object",
        "properties": {
            "childMessage": {
                "type": "string",
                "description": "Words to say before the child acts, e.g. open the door"
            },
            "isMessageAfterAction": {
                "type": "boolean",
                "description": "If true, the message is said after the action, e.g. the door is open"
            }
        },
        "required": ["childMessage", "isMessageAfterAction"]
    }
}
 */
