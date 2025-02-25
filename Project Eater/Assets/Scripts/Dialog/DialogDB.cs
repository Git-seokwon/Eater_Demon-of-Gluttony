using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogDB : ScriptableObject
{
    // ����Ʈ ������ �������� ���� ������ ��Ʈ �̸��� �����ؾ� �Ѵ�. 
    // �� ���� ��Ʈ�� 2�� �̻� ���� ���, ������ ��Ʈ �̸��� ����Ʈ ������ �����ϸ� �ȴ�. 
    public List<DialogDBEntity> Baal; // Replace 'EntityType' to an actual type that is serializable.
    public List<DialogDBEntity> Sigma; 
    public List<DialogDBEntity> Charles; 
    public List<DialogDBEntity> Tutorial; 
}
