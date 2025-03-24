using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : SingletonMonobehaviour<EventNotice>
{
    public delegate void RegisterDNAHandler(int tier, int index);
    public delegate void ResigterLatentSkillHandler(int index);
    // 도감 등록
    public delegate void RegisterMonsterDogam(Quest quest);

    public event RegisterDNAHandler onRegisterDNA;
    public event ResigterLatentSkillHandler onRegisterLatentSkill;
    public event RegisterMonsterDogam onRegisterMonsterDogam;

    public Image[] noticeImage;
    public TextMeshProUGUI[] noticeText;

    private int currentIndex = 0;

    private void OnEnable()
    {
        onRegisterDNA += NoticeRegisterDNA;
        onRegisterLatentSkill += NoticeRegisterLatentSkill;
        onRegisterMonsterDogam += NoticeRegisterMonsterDogam;
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

    public void OnRegisterMonsterDogam(Quest quest)
    {
        onRegisterMonsterDogam?.Invoke(quest);
    }

    private void NoticeRegisterMonsterDogam(Quest quest)
    {
        noticeImage[currentIndex].sprite = quest.Icon;
        noticeText[currentIndex].text = $"도감에 {quest.DisplayName}이 등록되었습니다.";
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseEvent", 3f);
    }

    private void NoticeRegisterDNA(int tier, int index)
    {
        var skill = GameManager.Instance.player.SkillSystem.SkillSlot[(tier, index)].Skill;

        noticeImage[currentIndex].sprite = skill.Icon;
        noticeText[currentIndex].text = "실험체의 힘을 흡수했습니다. 다음 진화부터 해당 스킬이 등장합니다";
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseEvent", 3f);
    }

    private void NoticeRegisterLatentSkill(int index)
    {
        var latentSkill = GameManager.Instance.player.LatentSkills[index].Skill[0];

        noticeImage[currentIndex].sprite = latentSkill.Icon;
        noticeText[currentIndex].text = "마인의 힘을 흡수했습니다. 강력한 스킬이 해방됩니다.";
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
