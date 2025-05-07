using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSelectorTooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI skillNameText;
    [SerializeField]
    private TextMeshProUGUI skillTypeText;
    [SerializeField]
    private TextMeshProUGUI skillGradeText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    // Tooltip을 보여주는 함수 
    public void Show(Skill skill)
    {
        skillNameText.text = skill.DisplayName;

        var skillTypeName = skill.Type == SkillType.Active ? "액티브" : "패시브";
        skillTypeText.text = $"[{skillTypeName}]";

        skillGradeText.text = GetSkillGradeString(skill);

        descriptionText.text = skill.Description;

        // Tooltip의 위치를 마우스 Position으로 위치한다. 
        transform.position = Input.mousePosition;

        // Tooltip Pivot 조정 
        // → Tool tip이 위치에 따라서 화면 밖으로 빠져나갈 수도 있다. 
        // Tooltip의 localPosition의 x 값이 0보다 크면 1이고, 아니면 0
        // ※ 0 : Canvas 화면 중앙
        // → Canvas 중앙에서 오른쪽으로 가면 양수 좌표가 되고 왼쪽으로 가면 음수 좌표가 된다. 
        // Ex) Tooltip이 음수 좌표에 있으면 Pivot은 0이 되고, UI는 우측 방향으로 UI가 그려진다. 
        //     반대로, Tooltip이 양수 좌표에 있으면 Pivot은 1이 되고, UI는 좌측 방향으로 그려진다. 
        float xPivot = transform.localPosition.x > 0f ? 1f : 0f;
        float yPivot = transform.localPosition.y > 0f ? 1f : 0f;

        // 정해준 Pivot을 Tooltip의 Pivot으로 Setting 해준다. 
        GetComponent<RectTransform>().pivot = new(xPivot, yPivot);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private string GetSkillGradeString(Skill skill)
    {
        string grade = "";

        switch (skill.Grade)
        {
            case SkillGrade.Common:
                grade = "Common";
                break;

            case SkillGrade.Rare:
                grade = "Rare";
                break;

            case SkillGrade.Unique:
                grade = "Unique";
                break;

            default:
                break;
        }

        return grade;
    }
}
