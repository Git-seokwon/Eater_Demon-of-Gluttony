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
    // 보상 확인
    public delegate void RegisterReward(QReward reward);

    public event RegisterDNAHandler onRegisterDNA;
    public event ResigterLatentSkillHandler onRegisterLatentSkill;
    public event RegisterMonsterDogam onRegisterMonsterDogam;
    public event RegisterReward onRegisterReward;

    public Image[] noticeImage;
    public TextMeshProUGUI[] noticeText;

    private int currentIndex = 0;

    private void OnEnable()
    {
        onRegisterDNA += NoticeRegisterDNA;
        onRegisterLatentSkill += NoticeRegisterLatentSkill;
        onRegisterMonsterDogam += NoticeRegisterMonsterDogam;
        onRegisterReward += NoticeRewardInfo;
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

    public void OnRegisterReward(QReward reward)
    {
        onRegisterReward?.Invoke(reward);
    }

    private void NoticeRegisterMonsterDogam(Quest quest)
    {
        noticeImage[currentIndex].sprite = quest.Icon;
        noticeText[currentIndex].text = $"도감에 {quest.DisplayName}이(가) 등록되었습니다.";
        noticeImage[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseEvent", 3f);
    }

    private void NoticeRewardInfo(QReward reward)
    {
        noticeImage[currentIndex].sprite = reward.Icon;
        noticeText[currentIndex].text = $"보상으로 {reward.name}을(를) 받았습니다.";
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
