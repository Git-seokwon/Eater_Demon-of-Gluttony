using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : SingletonMonobehaviour<EventNotice>
{
    public delegate void RegisterDNAHandler(int tier, int index);
    public delegate void ResigterLatentSkillHandler(int index);
    public event RegisterDNAHandler onRegisterDNA;
    public event ResigterLatentSkillHandler onRegisterLatentSkill;

    public Image[] noticeImage;
    public TextMeshProUGUI[] noticeText;

    private int currentIndex = 0;

    private void OnEnable()
    {
        onRegisterDNA += NoticeRegisterDNA;
        onRegisterLatentSkill += NoticeRegisterLatentSkill;
    }

    private void OnDisable()
    {
        onRegisterDNA -= NoticeRegisterDNA;
        onRegisterLatentSkill -= NoticeRegisterLatentSkill;
    }

    public void OnRegisterDNA(int tier, int index)
    {
        onRegisterDNA?.Invoke(tier, index);
    }

    public void OnResisterLatentSkill(int index)
    {
        onRegisterLatentSkill?.Invoke(index);
    }

    private void NoticeRegisterDNA(int tier, int index)
    {
        var skill = GameManager.Instance.player.SkillSystem.SkillSlot[(tier, index)].Skill;

        noticeImage[currentIndex].sprite = skill.Icon;
        noticeText[currentIndex].text = "����ü�� ���� ����߽��ϴ�. ���� ��ȭ���� �ش� ��ų�� �����մϴ�";
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseEvent", 3f);
    }

    private void NoticeRegisterLatentSkill(int index)
    {
        var latentSkill = GameManager.Instance.player.LatentSkills[index].Skill[0];

        noticeImage[currentIndex].sprite = latentSkill.Icon;
        noticeText[currentIndex].text = "������ ���� ����߽��ϴ�. ������ ��ų�� �ع�˴ϴ�.";
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseEvent", 3f);
    }

    private void CloseEvent()
    {
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(false);

        noticeImage[currentIndex].sprite = null;
        noticeText[currentIndex].text = "";
        currentIndex = (currentIndex + 1) % noticeImage.Length;
    }
}
