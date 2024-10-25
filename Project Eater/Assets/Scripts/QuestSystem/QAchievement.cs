using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/QAchievement", fileName ="QAchievement_")]

public class QAchievement : Quest
{
    public override bool IsSavable => true;
    public override bool IsCancelable => false;

    public override void Cancel()
    {
        Debug.LogAssertion("업적은 해제할 수 없습니다.");
    }
}
