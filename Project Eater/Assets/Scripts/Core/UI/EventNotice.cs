using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : SingletonMonobehaviour<EventNotice>
{
    public delegate void RegisterDNAHandler(int tier, int index);
    public event RegisterDNAHandler onRegisterDNA;

    public Image[] noticeUI;

    private int currentIndex = 0;

    private void OnEnable()
    {
        onRegisterDNA += NoticeRegisterDNA;
    }

    private void OnDisable()
    {
        onRegisterDNA -= NoticeRegisterDNA;
    }

    public void OnRegisterDNA(int tier, int index)
    {
        onRegisterDNA?.Invoke(tier, index);
    }

    public void NoticeRegisterDNA(int tier, int index)
    {
        var skill = GameManager.Instance.player.SkillSystem.SkillSlot[(tier, index)].Skill;

        noticeUI[currentIndex].sprite = skill.Icon;

        noticeUI[currentIndex].gameObject.transform.parent.gameObject.SetActive(true);

        Invoke("CloseRegisterDNA", 3f);
    }

    private void CloseRegisterDNA()
    {
        noticeUI[currentIndex].gameObject.transform.parent.gameObject.SetActive(false);

        noticeUI[currentIndex].sprite = null;
        currentIndex = (currentIndex + 1) % noticeUI.Length;
    }
}
