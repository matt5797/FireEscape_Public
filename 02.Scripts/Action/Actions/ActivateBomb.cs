using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using FireEscape.Ability;
using FireEscape.Enviroment;

namespace FireEscape.Action
{
    public class ActivateBomb : AIAction
    {
        public float MinimumDistance = 1f;
        public float activationDistance = 20f;
        public Transform evacuationPos;

        Character _player;
        protected Vector3 _directionToTarget;
        protected int _numberOfJumps = 0;
        protected Vector2 _movementVector;

        protected CharacterPathfinder3D _characterPathfinder3D;

        protected override void Initialization()
        {
            ActionID = 9991;
            name = "Activate_Bomb";
            description = "Activate the bomb";
        }

        public override bool CanExecute()
        {
            if (_player != LevelManager.Instance.Players[0].GetComponent<Character>())
            {
                _player = LevelManager.Instance.Players[0].GetComponent<Character>();
                _characterPathfinder3D = _player.FindAbility<CharacterPathfinder3D>();
            }

            // Implement logic to check if the player is close enough to the bomb to activate it
            // return Vector3.Distance(_player.transform.position, transform.position) <= activationDistance;
            return true;
        }

        public override IEnumerator Execute()
        {
            // 1. The player moves toward the bomb.
            // (assuming you have movement logic elsewhere, which you can call here)
            _characterPathfinder3D.SetNewDestination(transform);

            while (Vector3.Distance(_player.transform.position, transform.position) > MinimumDistance)
            {
                yield return null;
            }

            // 2. Activate the bomb when the player gets close enough to it.
            StartCoroutine(ActivateAndDetonateBomb());

            _characterPathfinder3D.SetNewDestination(evacuationPos);

            while (Vector3.Distance(_player.transform.position, evacuationPos.position) > MinimumDistance)
            {
                yield return null;
            }
        }

        private IEnumerator ActivateAndDetonateBomb()
        {
            // 3. The bomb will detonate itself after 5 seconds and briefly rise into the air.
            yield return new WaitForSeconds(3);
            Rigidbody bombRigidbody = GetComponent<Rigidbody>();
            bombRigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse); // Pushing bomb up in the air
                                                                       // Implement explosion logic here (like playing explosion animation, sound, etc.)
        }

        public override void OnEnterAction()
        {
            base.OnEnterAction();
            // Implement additional logic when action starts
        }

        public override void OnExitAction()
        {
            base.OnExitAction();
            // Implement additional logic when action ends
        }

        public override void Cancel()
        {
            // Implement logic to cancel the action
        }
    }
}