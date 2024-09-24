
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireEscape.Object;
using MoreMountains.TopDownEngine;
using FireEscape.Ability;

namespace FireEscape.Action
{
    public class RaiseUp : AIAction
    {
        Character _player;

        protected CharacterCrouch _characterCrouch;

        protected override void Initialization()
        {
            ActionID = 12;
            name = "RaiseUp";
            description = "RaiseUp";
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
                _characterCrouch = _player.FindAbility<CharacterCrouch>();
            }

            return true;
        }

        public override IEnumerator Execute()
        {
            _characterCrouch.StopForcedCrouch();

            yield return null;
        }
    }
}