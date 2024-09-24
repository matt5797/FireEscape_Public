using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWettingCloth : CharacterAbility
{
    private Character character;

    private void Start()
    {
        character = GetComponentInParent<Character>();
    }
    public void WettingCloth()
    {
        print("WettingCloth");
        character._animator.SetTrigger("WettingCloth");
    }
}
