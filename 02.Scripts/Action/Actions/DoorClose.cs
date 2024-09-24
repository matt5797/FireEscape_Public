using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireEscape.Object;
using MoreMountains.TopDownEngine;
using FireEscape.Ability;
using FireEscape.Enviroment;

namespace FireEscape.Action
{
    public class DoorClose : AIAction
    {
        Door _door;
        Character _player;

        public Transform doorknobPosition;
        public float MinimumDistance = 0.1f;

        protected CharacterMovement _characterMovement;

        protected Vector3 _directionToTarget;
        protected Vector2 _movementVector;

        protected override void Initialization()
        {
            ActionID = 102;
            name = "Close_Door";
            description = "Close the Opened door";
        }

        private void Awake()
        {
            _door = GetComponent<Door>();
        }

        public override void Cancel()
        {
            if (_door.isMoving)
            {
                _door.isMoving = false;
            }
        }

        public override bool CanExecute()
        {
            if (_player != LevelManager.Instance.Players[0].GetComponent<Character>())
            {
                _player = LevelManager.Instance.Players[0].GetComponent<Character>();
                _characterMovement = _player.FindAbility<CharacterMovement>();
            }

            return _door.IsOpen == true && _door.isMoving == false;
        }

        public override IEnumerator Execute()
        {
            while (DistanceInVector2(_player.transform.position, doorknobPosition.position) > MinimumDistance)
            {
                _directionToTarget = doorknobPosition.position - _player.transform.position;
                _movementVector.x = _directionToTarget.x;
                _movementVector.y = _directionToTarget.z;
                _characterMovement.SetMovement(_movementVector);
                yield return null;
            }
            _characterMovement.SetMovement(Vector2.zero);

            _door.isMoving = true;
            float t = 0;
            float angle = _door.transform.localEulerAngles.y;
            while (t < _door.OpenTime)
            {
                t += Time.deltaTime;
                _door.transform.localEulerAngles = new Vector3(_door.transform.localEulerAngles.x, Mathf.LerpAngle(angle, _door.CloseAngle, t), _door.transform.localEulerAngles.z);
                yield return null;
            }
            _door.IsOpen = false;
            _door.isMoving = false;
        }

        public float DistanceInVector2(Vector3 v1, Vector3 v2)
        {
            return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.z - v2.z, 2));
        }
    }
}