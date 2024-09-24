using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class CharacterCoveringFace : CharacterAbility
{
    private Character character;
    public bool isCoverFace = false;
    public GameObject JWtowel;
    public PickUp pickUpTowel;

    private void Start()
    {
        character = GetComponentInParent<Character>();
    }
    public void CoveringFace()
    {
        StartCoroutine(IE_CoveringFace());
    }

    public IEnumerator IE_CoveringFace()
    {
        character._animator.CrossFade("CoverFace_Start", 1f, 1, 0.1f);

        yield return new WaitForSeconds(0.9f);

        if(pickUpTowel.isPickUp == true)
        {
            JWtowel.SetActive(true);
        }

        isCoverFace = true;
    }
}
