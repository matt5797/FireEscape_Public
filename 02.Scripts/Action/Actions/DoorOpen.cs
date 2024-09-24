using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using FireEscape.Object;
using FireEscape.Enviroment;
using FireEscape.Ability;

namespace FireEscape.Action
{
    public class DoorOpen : AIAction
    {
        Door _door;
        Character _player;

        public Transform doorknobPosition;
        public float MinimumDistance = 0.2f;

        protected CharacterMovement _characterMovement;
        protected CharacterPathfinder3D _characterPathfinder3D;
        protected Vector3 _directionToTarget;
        protected Vector2 _movementVector;

        protected CharacterCoveringFace _characterCoveringFace;
        protected CharacterCrouch _characterCrouch;
        public GameObject JWtowel;

        protected override void Initialization()
        {
            ActionID = 101;
            name = "Open_Door";
            description = "Open the Closed door";
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
                _characterPathfinder3D = _player.FindAbility<CharacterPathfinder3D>();
                _characterCoveringFace = _player.FindAbility<CharacterCoveringFace>();
                _characterCrouch = _player.FindAbility<CharacterCrouch>();
            }

            bool isPlayerInRoom = false;
            /*if (_door.rooms != null)
            {
                for (int i = 0; i < _door.rooms.Count; i++)
                {
                    if (_door.rooms[i].IsPlayerInRoom())
                    {
                        isPlayerInRoom = true;
                    }
                }
            }*/
            isPlayerInRoom = true;

            return isPlayerInRoom==true && _door.IsLocked == false && _door.IsOpen == false && _door.isMoving == false;
        }

        public override IEnumerator Execute()
        {
            _characterPathfinder3D.SetNewDestination(doorknobPosition);

            while (DistanceInVector2(_player.transform.position, doorknobPosition.position) > MinimumDistance)
            {
                yield return null;
            }

            _characterPathfinder3D.Target = null;
            _characterMovement.SetMovement(Vector2.zero);

            //======== animation_Start ===================

            _player._animator.Play("Idle", 1, 0f);
            _player._animator.Play("Idle", 2, 0f);
            JWtowel.SetActive(false);
            _player._animator.SetTrigger("DoorOpen");

            yield return new WaitForSeconds(0.5f);

            if (_characterCoveringFace.isCoverFace == true)
            {
                _characterCoveringFace.CoveringFace();
            }

            if(_characterCrouch.isCrouch == true)
            {
                _characterCrouch.StartForcedCrouch();
            }

            //======== animation_End =====================

            _door.isMoving = true;
            float t = 0;
            float angle = _door.DoorObject.transform.localEulerAngles.y;
            while (t < _door.OpenTime)
            {
                t += Time.deltaTime;
                _door.DoorObject.transform.localEulerAngles = new Vector3(_door.DoorObject.transform.localEulerAngles.x, Mathf.LerpAngle(angle, _door.OpenAngle, t), _door.DoorObject.transform.localEulerAngles.z);
                yield return null;
            }
            _door.IsOpen = true;
            _door.isMoving = false;

            //yield return new WaitForSeconds(0.5f);
        }

        public float DistanceInVector2(Vector3 v1, Vector3 v2)
        {
            return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.z - v2.z, 2));
        }

        public void Anim()
        {

        }
    }
}