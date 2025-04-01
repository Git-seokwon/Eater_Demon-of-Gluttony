using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : OptionUIBase
{
    [SerializeField]
    private GameObject SystemWindow;
    [SerializeField]
    private GameObject SettingBG;
    [SerializeField]
    private GameObject ExplainBG;
    [SerializeField]
    private Button explainBtn;
    [SerializeField]
    private TMP_Text explainBtnText;

    protected override void Awake()
    {
        base.Awake();
        explainBtn.onClick.AddListener(OnClickExplain);

        OnClickOption();
    }

    public override void OnClickOption()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
        SystemWindow.SetActive(!SystemWindow.activeSelf);
    }

    protected override void OnClickConfirm()
    {
        OnClickOption();
        ConfirmSettingAction?.Invoke();
    }

    protected override void OnClickCancel()
    {
        OnClickOption();
        CancelSettingAction?.Invoke();
    }

    private void OnClickExplain()
    {
        ExplainBG.SetActive(!ExplainBG.activeSelf);
        SettingBG.SetActive(!SettingBG.activeSelf);

        if (explainBtnText.text == "조작법 보기")
            explainBtnText.text = "설정 보기";
        else
            explainBtnText.text = "조작법 보기";
    }
}
