using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireEscape.Chat
{
    public class SurroundingObject
    {
        public string ObjectType { get; set; }
        public string Content { get; set; }

        public SurroundingObject() { }

        public SurroundingObject(string objectType, string content)
        {
            ObjectType = objectType;
            Content = content;
        }
    }
}
