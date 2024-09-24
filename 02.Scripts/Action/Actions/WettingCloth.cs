using FireEscape.Ability;
using FireEscape.Action;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WettingCloth : AIAction
{
    Character _player;

    public Transform washstandPosition;
    public float MinimumDistance = 0.2f;

    protected CharacterMovement _characterMovement;
    protected CharacterPathfinder3D _characterPathfinder3D;
    protected Vector3 _directionToTarget;
    protected Vector2 _movementVector;

    protected CharacterCoveringFace _characterCoveringFace;
    protected CharacterWettingCloth _characterWettingCloth;
    private float moveSpeed = 0.01f;
    private float minDistance = 0.15f;

    public PickUp PickUpTowel;
    public GameObject JWTowel;


    protected override void Initialization()
    {
        ActionID = 10001;
        name = "WettingCloth";
        description = "WettingCloth";
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
            _characterWettingCloth = _player.FindAbility<CharacterWettingCloth>();
            _characterCoveringFace = _player.FindAbility<CharacterCoveringFace>();

        }

        return true;
    }

    public override IEnumerator Execute()
    {
        _characterPathfinder3D.SetNewDestination(washstandPosition);

        while (DistanceInVector2(_player.transform.position, washstandPosition.position) > MinimumDistance)
        {
            /*if (DistanceInVector2(_player.transform.position, washstandPosition.position) < 0.25)
            {
                // 강제이동
                while (DistanceInVector2(_player.transform.position, washstandPosition.position) > minDistance)
                {
                    // 플레이어를 목표 방향으로 조금씩 이동
                    _player.transform.position = Vector3.MoveTowards(_player.transform.position, washstandPosition.position, moveSpeed * Time.deltaTime * Time.deltaTime);
                }
            }*/

            yield return null;
        }

        _characterPathfinder3D.Target = null;
        _characterMovement.SetMovement(Vector2.zero);

        _player._animator.Play("Idle", 1, 0f);
        _player._animator.Play("Idle", 2, 0f);
        _player._animator.SetTrigger("WettingCloth");
        JWTowel.SetActive(false);

        yield return new WaitForSeconds(0.9f);
        if (PickUpTowel.isPickUp == true)
        {
            JWTowel.SetActive(true);
        }

        yield return new WaitForSeconds(2.6f);

        if (_characterCoveringFace.isCoverFace == true)
        {
            _characterCoveringFace.CoveringFace();
        } else
        {
            JWTowel.SetActive(false);

        }

        yield return new WaitForSeconds(1.25f);

    }
    public float DistanceInVector2(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.z - v2.z, 2));
    }
}
