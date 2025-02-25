using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogDB : ScriptableObject
{
    // 리스트 변수의 변수명은 엑셀 파일의 시트 이름과 동일해야 한다. 
    // → 만약 시트가 2개 이상 있을 경우, 각각의 시트 이름을 리스트 변수로 선언하면 된다. 
    public List<DialogDBEntity> Baal; // Replace 'EntityType' to an actual type that is serializable.
    public List<DialogDBEntity> Sigma; 
    public List<DialogDBEntity> Charles; 
    public List<DialogDBEntity> Tutorial; 
}
