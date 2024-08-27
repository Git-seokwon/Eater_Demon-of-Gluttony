using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("isStuning");

    public override string Description => "����";
    protected override int AnimationHash => kAnimationHash;
}
