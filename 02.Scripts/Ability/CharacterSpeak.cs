using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using FireEscape.Chat;
using FireEscape.Network;
using TMPro;
using UnityEngine.UI;

namespace FireEscape.Ability
{
    public class CharacterSpeak : CharacterAbility
    {
        public TextMeshProUGUI speakUI;
        public GameObject textBubble;

        public float speakTime = 3f;

        public void Speak(string message)
        {
            StartCoroutine(SetActiveSpeakUI(message));
        }

        private IEnumerator SetActiveSpeakUI(string message)
        {
            textBubble?.SetActive(true);

            speakUI.text = message;

            yield return new WaitForSeconds(speakTime);

            textBubble?.SetActive(false);
        }
    }
}