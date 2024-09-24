using FireEscape.Ability;
using FireEscape.Action;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveringFace : AIAction
{
    Character _player;

    protected CharacterCoveringFace _characterCoveringFace;


    protected override void Initialization()
    {
        ActionID = 10000;
        name = "CoveringFace";
        description = "CoveringFace";
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
            _characterCoveringFace = _player.FindAbility<CharacterCoveringFace>();
        }

        return true;
    }

    public override IEnumerator Execute()
    {
        _characterCoveringFace.CoveringFace();

        yield return new WaitForSeconds(1.25f);

    }
}
