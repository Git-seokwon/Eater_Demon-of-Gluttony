using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionUI : MonoBehaviour
{
    [SerializeField] private string codeName;
    [SerializeField] private string actionKey;

    public string CodeName => codeName;
    public string ActionKey => actionKey;

    // �� �ʿ��Ѱ� �ֳ�
}
