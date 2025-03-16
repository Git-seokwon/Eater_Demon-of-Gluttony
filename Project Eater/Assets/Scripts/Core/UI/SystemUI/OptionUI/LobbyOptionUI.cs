using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOptionUI : OptionUIBase
{
    [SerializeField]
    private GameObject GameStart;
    [SerializeField]
    private GameObject Option;
    [SerializeField]
    private GameObject Exit;

    public override void OnClickOption()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
        GameStart.SetActive(!GameStart.activeSelf);
        Option.SetActive(!Option.activeSelf);
        Exit.SetActive(!Exit.activeSelf);
    }
}
