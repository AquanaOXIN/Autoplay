using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField]
    private Characters myCharacter = default;
    [SerializeField]
    private Animator myAnimator = default;

    public void PlayAnimation(int num)
    {
        if (myCharacter.GetCurrentState() == num)
            return;
        myAnimator.Play(myCharacter.animStateName[num]);
        myCharacter.SetCurrentState(num);
    }
}
