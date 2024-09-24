using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireEscape.Chat
{
    public class PlayerOptions
    {
        public List<string> Options { get; private set; }

        public PlayerOptions(List<string> options)
        {
            Options = options;
        }
        
        public void SelectOption(int index)
        {
            // Logic for selecting an option
        }
    }
}
