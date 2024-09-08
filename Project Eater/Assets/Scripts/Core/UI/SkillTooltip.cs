using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SkillTooltip : SingletonMonobehaviour<SkillTooltip>
{
    [SerializeField]
    private TextMeshProUGUI displayNameText;
    [SerializeField]
    private TextMeshProUGUI skillTypeText;
    [SerializeField]
    private TextMeshProUGUI skillGradeText;
    [SerializeField]
    private TextMeshProUGUI cooldownText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    // ���� ���ڿ����� �����ϱ� ���� StringBuilder ���� 
    // �� StringBuilder : https://hongjinhyeon.tistory.com/91
    private StringBuilder stringBuilder = new();

    private void Start() => gameObject.SetActive(false);

    // Tooltip�� ���콺 Position�� ���󰡵��� ���ش�. 
    private void Update() => transform.position = HelperUtilities.GetMouseWorldPosition();

    // Tooltip�� �����ִ� �Լ� 
    public void Show(Skill skill)
    {
        displayNameText.text = skill.DisplayName;

        var skillTypeName = skill.Type == SkillType.Active ? "��Ƽ��" : "�нú�";
        skillTypeText.text = $"[{skillTypeName}]";

        // ��ų�� Cooldown�� �Ҽ��� �� ° �ڸ����� ǥ�õǰ� ���ڿ��� ���� Setting
        cooldownText.text = $"���� ��� �ð� : {skill.Cooldown:0.##}��";

        descriptionText.text = skill.Description;

        // Tooltip�� ��ġ�� ���콺 Position���� ��ġ�Ѵ�. 
        transform.position = HelperUtilities.GetMouseWorldPosition();

        // Tooltip Pivot ���� 
        // �� Tool tip�� ��ġ�� ���� ȭ�� ������ �������� ���� �ִ�. 
        // Tooltip�� localPosition�� x ���� 0���� ũ�� 1�̰�, �ƴϸ� 0
        // �� 0 : Canvas ȭ�� �߾�
        // �� Canvas �߾ӿ��� ���������� ���� ��� ��ǥ�� �ǰ� �������� ���� ���� ��ǥ�� �ȴ�. 
        // Ex) Tooltip�� ���� ��ǥ�� ������ Pivot�� 0�� �ǰ�, UI�� ���� �������� UI�� �׷�����. 
        //     �ݴ��, Tooltip�� ��� ��ǥ�� ������ Pivot�� 1�� �ǰ�, UI�� ���� �������� �׷�����. 
        float xPivot = transform.localPosition.x > 0f ? 1f : 0f;
        float yPivot = transform.localPosition.y > 0f ? 1f : 0f;

        // ������ Pivot�� Tooltip�� Pivot���� Setting ���ش�. 
        GetComponent<RectTransform>().pivot = new(xPivot, yPivot);

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
