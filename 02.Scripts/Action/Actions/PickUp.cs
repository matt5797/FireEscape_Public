using FireEscape.Action;
using FireEscape.Object;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : AIAction
{
    Character _player;

    //public Transform handkerchiefPosition;
    public float MinimumDistance = 0.2f;

    protected CharacterMovement _characterMovement;
    protected CharacterPathfinder3D _characterPathfinder3D;
    protected Vector3 _directionToTarget;
    protected Vector2 _movementVector;

    protected CharacterCoveringFace _characterCoveringFace;
    protected CharacterCrouch _characterCrouch;

    private float moveSpeed = 0.01f;
    private float minDistance = 0.15f;

    public bool isPickUp = false;

    protected override void Initialization()
    {
        ActionID = 10002;
        name = "PickUp";
        description = "PickUp the towel";
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
            _characterMovement = _player.FindAbility<CharacterMovement>();
            _characterPathfinder3D = _player.FindAbility<CharacterPathfinder3D>();
            _characterCrouch = _player.FindAbility<CharacterCrouch>();
        }
        return true;
    }

    public override IEnumerator Execute()
    {
        _characterPathfinder3D.SetNewDestination(this.transform);

        while (DistanceInVector2(_player.transform.position, this.transform.position) > MinimumDistance)
        {
           /* if (DistanceInVector2(_player.transform.position, this.transform.position) < 0.25)
            {
                // 강제이동
                while (DistanceInVector2(_player.transform.position, this.transform.position) > minDistance)
                {
                    // 플레이어를 목표 방향으로 조금씩 이동
                    _player.transform.position = Vector3.MoveTowards(_player.transform.position, this.transform.position, moveSpeed * Time.deltaTime * Time.deltaTime);
                }
            }*/
            yield return null;
        }

        _characterPathfinder3D.Target = null;
        _characterMovement.SetMovement(Vector2.zero);

        //======== animation_Start ===================

        _player._animator.Play("Idle", 1, 0f);
        _player._animator.Play("Idle", 2, 0f);

        _player._animator.SetTrigger("PickUp");

        yield return new WaitForSeconds(0.75f);

        this.gameObject.SetActive(false);
        isPickUp = true;


        // PickUp 대기시간 추가
        if (_characterCrouch.isCrouch == true)
        {
            _characterCrouch.StartForcedCrouch();
        }

        //======== animation_End =====================

        // PickUp anim time으로 수정(만약 Couch 필요하면 crouch 대기시간으로 변경)
        yield return new WaitForSeconds(1f);
    }

    public float DistanceInVector2(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.z - v2.z, 2));
    }


}
