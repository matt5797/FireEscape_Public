using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireEscape.Object;
using MoreMountains.TopDownEngine;
using FireEscape.Ability;
using TMPro;

namespace FireEscape.Action
{
    public class Speaking : AIAction
    {
        Character _player;

        protected CharacterSpeak _characterSpeak;


        protected override void Initialization()
        {
            ActionID = 01;
            name = "Speaking";
            description = "Speaking";
        }

        private void Awake()
        {

        }

        public override void Cancel()
        {

        }

        public override bool CanExecute()
        {
            if (_player != LevelManager.Instance.Players[0].GetComponent<Character>())
            {
                _player = LevelManager.Instance.Players[0].GetComponent<Character>();
                _characterSpeak = _player.FindAbility<CharacterSpeak>();
            }

            return true;
        }

        public override IEnumerator Execute()
        {
            // Implement logic to make the character speak
            //_characterSpeak.Speak(message);

            yield return null;
        }
    }
}