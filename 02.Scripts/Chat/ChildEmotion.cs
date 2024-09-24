using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireEscape.Chat
{
    public class ChildEmotion
    {
        public int AnxietyLevel { get; set; }

        public ChildEmotion() { }

        public ChildEmotion(int anxietyLevel)
        {
            AnxietyLevel = anxietyLevel;
        }
    }
}
