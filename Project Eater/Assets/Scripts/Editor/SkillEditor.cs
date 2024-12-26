using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal.Internal;

[CustomEditor(typeof(Skill))]
public class SkillEditor : IdentifiedObjectEditor
{
    private SerializedProperty typeProperty;
    private SerializedProperty useTypeProperty;
    private SerializedProperty gradeProperty;
    private SerializedProperty stackCountDisplayroperty;

    private SerializedProperty movementProperty;
    private SerializedProperty executionTypeProperty;

    private SerializedProperty targetSelectionTimingOptionProperty;
    private SerializedProperty targetSearchTimingOptionProperty;

    private SerializedProperty useConditionsProperty;

    private SerializedProperty maxLevelProperty;
    private SerializedProperty defaultLevelProperty;
    private SerializedProperty skillDatasProperty;

    // Toolbar Button들의 이름 : "Cast", "Charge", "Preceding", "Action"
    // 1) Cast   : customActionsOnCast 변수가 그려짐 
    // 2) Charge : customActionsOnCharge 변수가 그려짐 
    // ... 
    // → 각 상태에서 CustomActions을 그릴 수 있음 (가독성)
    private readonly string[] customActionsToolbarList = new[] { "Cast", "Charge" };
    private readonly string[] currentCustomActionsToolbarList = new[] { "Preceding", "Action" };

    // Skill Data마다 선택한 Toolbar Button의 Index 값
    // → Level Data마다 customActions가 존재하는데 1 Level에서는 Cast, 2 Level에서는 Charge를 선택할 수도 있다. 
    //    이렇게 Data마다 몇 번째 Toolbar Button을 선택했는지 기록하는 변수 
    // ※ Key   : Data Level
    // ※ Value : 선택된 Button의 Index
    private Dictionary<int, int> customActionToolbarIndexesByLevel = new();

    // ※ Key   : Apply Count Action
    // ※ Value : 선택된 Button의 Index
    private Dictionary<int, int> customActionToolbarIndexesByApplyCount = new();

    private bool IsPassive => typeProperty.enumValueIndex == (int)SkillType.Passive;
    private bool IsToggleType => useTypeProperty.enumValueIndex == (int)SkillUseType.Toggle;

    // Toggle, Passive Type일 때는 사용하지 않는 변수들을 보여주지 않을 것임
    // → IsToggleType, IsPassive 모두 false인 경우에만 IsDrawPropertyAll가 true가 되어 모든 Property들을 그려줄 것
    private bool lsDrawPropertyAll => !IsToggleType && !IsPassive;

    protected override void OnEnable()
    {
        base.OnEnable();

        // FindProperty로 Skill의 변수들을 가져온다. 
        typeProperty = serializedObject.FindProperty("type");
        gradeProperty = serializedObject.FindProperty("grade");
        useTypeProperty = serializedObject.FindProperty("useType");
        stackCountDisplayroperty = serializedObject.FindProperty("stackCountDisplay");

        movementProperty = serializedObject.FindProperty("movement");
        executionTypeProperty = serializedObject.FindProperty("executionType");

        targetSelectionTimingOptionProperty = serializedObject.FindProperty("targetSelectionTimingOption");
        targetSearchTimingOptionProperty = serializedObject.FindProperty("targetSearchTimingOption");

        useConditionsProperty = serializedObject.FindProperty("useConditions");

        maxLevelProperty = serializedObject.FindProperty("maxLevel");
        defaultLevelProperty = serializedObject.FindProperty("defaultLevel");
        skillDatasProperty = serializedObject.FindProperty("skillDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        float prevLabelWidth = EditorGUIUtility.labelWidth;
        // Label의 넓이를 변수명이 길어도 잘리지 않고 보이도록 220으로 늘린다.
        EditorGUIUtility.labelWidth = 260f;

        // 각 항목들을 그려준다. 
        DrawSettings();
        DrawUseConditions();
        DrawSkillDatas();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    // ※ DrawSettings : Skill의 Enum 변수들을 그리는 함수 
    private void DrawSettings()
    {
        // Setting foldout 버튼을 그리고 펼쳐지지 않았으면 return 
        if (!DrawFoldoutTitle("Setting"))
            return;

        // ※ typeProperty : Passive or Active
        CustomEditorUtility.DrawEnumToolbar(typeProperty);
        // ※ gradeProperty : Common, Rare, Unique
        CustomEditorUtility.DrawEnumToolbar(gradeProperty);
        // ※ movementProperty : Move, Stop
        CustomEditorUtility.DrawEnumToolbar(movementProperty);
        EditorGUILayout.PropertyField(stackCountDisplayroperty);

        if (!IsPassive)
            // Active 스킬이면 Instant or Toggle Enum Button을 그림 
            CustomEditorUtility.DrawEnumToolbar(useTypeProperty);
        else
            // instant로 고정
            // → Passive 스킬은 무조건 Instant 스킬로 설정 
            // → Passive 스킬은 Toggle Type이 될 수 없다.
            useTypeProperty.enumValueIndex = 0;

        // Passive or Toggle 타입이 아니라면 
        if (lsDrawPropertyAll)
        {
            EditorGUILayout.Space();
            CustomEditorUtility.DrawUnderLine();
            EditorGUILayout.Space();

            // executionType과 applyType을 그려준다. 
            CustomEditorUtility.DrawEnumToolbar(executionTypeProperty);
        }
        //  Passive or Toggle 타입이라면 executionType 값을 0으로 고정 
        // 1) executionType : Auto
        else
            executionTypeProperty.enumValueIndex = 0;

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderLine();
        EditorGUILayout.Space();

        CustomEditorUtility.DrawEnumToolbar(targetSelectionTimingOptionProperty);
        CustomEditorUtility.DrawEnumToolbar(targetSearchTimingOptionProperty);
    }

    // ※ DrawUseConditions : UseConditions Property 그려주는 함수 
    private void DrawUseConditions()
    {
        // DrawFoldoutTitle Title이 펼쳐지지 않으면 return  
        if (!DrawFoldoutTitle("Use Condition"))
            return;

        EditorGUILayout.PropertyField(useConditionsProperty);
    }

    private void DrawSkillDatas()
    {
        // Skill의 Data가 아무것도 존재하지 않으면(Data의 길이가 0이라면) 1개(1 Level Data)를 자동적으로 만들어줌
        if (skillDatasProperty.arraySize == 0)
        {
            // 배열 길이를 늘려서 새로운 Element를 생성
            skillDatasProperty.arraySize++;
            // 추가한 Data의 Level(SkillData[0].level)을 1로 설정
            skillDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        // Data Title이 펼쳐지지 않으면 return 
        if (!DrawFoldoutTitle("Data"))
            return;

        // ※ MaxLevel을 level 상한으로 고정 시키는 작업
        // Property를 수정하지 못하게 GUI Enable의 false로 바꿈
        GUI.enabled = false;
        var lastIndex = skillDatasProperty.arraySize - 1;
        // 마지막 SkillData(= 가장 높은 Level의 Data)를 가져옴
        var lastSkillData = skillDatasProperty.GetArrayElementAtIndex(lastIndex);
        // maxLevel을 마지막 Data의 Level로 고정
        maxLevelProperty.intValue = lastSkillData.FindPropertyRelative("level").intValue;
        // maxLevel Property를 그려줌
        EditorGUILayout.PropertyField(maxLevelProperty);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(defaultLevelProperty);

        // SkillDatas 순회
        for (int i = 0; i < skillDatasProperty.arraySize; i++)
        {
            // 현재 Index의 SkillData 가져오기 
            var property = skillDatasProperty.GetArrayElementAtIndex(i);

            // applyActions은 개별 작업이 필요하기 때문에 가져옴 
            var skillApplyActionProperty = property.FindPropertyRelative("applyActions");
            var applyCount = property.FindPropertyRelative("applyCount");
            // isUseCast ~ needChargeTimeToUse는 개별 작업이 필요하기 때문에 일일이 가져옴
            var isUseCastProperty = property.FindPropertyRelative("isUseCast");
            var isUseChargeProperty = property.FindPropertyRelative("isUseCharge");
            var chargeDurationProperty = property.FindPropertyRelative("chargeDuration");
            var chargeTimeProperty = property.FindPropertyRelative("chargeTime");
            var needChargeTimeToUseProperty = property.FindPropertyRelative("needChargeTimeToUse");

            // HelpBox 그려주기 
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data의 Level과 Data 삭제를 위한 X Button을 그려주는 Foldout Title을 그려줌
                // 단, 첫번째 Data(= index 0) 지우면 안되기 때문에 X Button을 그려주지 않음 (i != 0)
                // X Button을 눌러서 Data가 지워지면 true를 return 함
                if (DrawRemovableLevelFoldout(skillDatasProperty, property, i, i != 0))
                {
                    // Data가 삭제되었으며 더 이상 GUI를 그리지 않고 바로 빠져나감
                    // 다음 Frame에 처음부터 다시 그리기 위함
                    EditorGUILayout.EndVertical();
                    break;
                }

                // 들여쓰기 1칸 
                // → Foldout 창의 level 문구와 같은 라인에서 그려지도록 하기 위함 
                EditorGUI.indentLevel += 1;

                if (property.isExpanded)
                {
                    #region SerializedProperty와 Iterator
                    // → SerializedProperty는 단순히 변수가 아니라 반복자라고 부르는 Iterator이다. 
                    //    Iterator는 쉽게 말해 내부 값을 순회할 수 있게 해주는 객체이다. 
                    //    여기서 내부 값이라는 건 찾아온 변수를 포함해서 나머지 모든 Serialize 변수를 의미한다. 
                    // Ex) FindProperty로 type 변수를 찾아 왔을 때, 그 변수 하나로 다른 모든 변수에도 접근을 할 수 있다. 

                    // ※ Next, NextVisible 함수 
                    // → 다른 변수에 접근하는 방법이 바로, SerialzeProperty의 Next 혹은 NextVisible 함수이다. 
                    // Ex) type Property가 NextVisible 함수를 사용하게 되면 type Property의 값이 type 변수의 바로 아래 선언된 
                    //     다음 변수인 useType 변수가 된다. 이 상태에서 PropertyField 함수로 typeProperty를 그려주게 되면
                    //     useType이 그려지게 된다. 즉, NextVisible 함수를 쓸때마다 SerializeProperty의 값이 그 다음 변수로 넘어가게 된다. 

                    // ※ Next 함수와 NextVisible 함수의 차이점
                    // NextVisible 함수는 HideInInspector Attribute가 달려있는 Serialize 변수는 건너뛴다. 

                    // ※ NextVisible 함수 
                    // 1) 반환값 bool : 더 이상 이동할 변수가 없으면 false를 반환한다. 
                    // → while 문을 이용하여 NextVisible 함수가 false를 반환할 때까지 이동하면서 PropertyField 함수로 변수를 그리는 방식이 있음
                    // 2) 인자값 bool : 현재 변수 Type이 Class나 Struct라면 그 변수 내부로 들어갈 지 여부  
                    // Ex) property(Skill Data)의 내부 Serialize 변수(Level)로 이동
                    #endregion

                    // SkillData Property 내부로 들어감 -> Property가 SkillData의 level field가 된다.
                    property.NextVisible(true);

                    // Level Property를 그려주기
                    // → Level 변수는 수정할 수 없다. 
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(property);
                    GUI.enabled = true;

                    // SkillRunningFinishOption 그려주기 
                    property.NextVisible(true);
                    CustomEditorUtility.DrawEnumToolbar(property);

                    // 1) duration
                    // 2) applyCount
                    // 3) applyCycle
                    for (int j = 0; j < 3; j++)
                    {
                        // 다음 변수의 Property로 이동하면서 그려줌
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }
                    EditorGUILayout.Space();
                    CustomEditorUtility.DrawUnderLine();
                    property.NextVisible(false);

                    if (skillApplyActionProperty.arraySize != applyCount.intValue)
                    {
                        skillApplyActionProperty.arraySize = applyCount.intValue;

                        for (int j = 0; j < skillApplyActionProperty.arraySize; j++)
                            skillApplyActionProperty.GetArrayElementAtIndex(j).FindPropertyRelative("currentApplyCount").intValue = j + 1;
                    }

                    EditorGUI.indentLevel += 1;

                    // Skill Apply Action 순회 
                    for (int j = 0; j < skillApplyActionProperty.arraySize; j++)
                    {
                        // 현재 Apply Count의 Skill Apply Action 가져오기 
                        var applyActionProperty = skillApplyActionProperty.GetArrayElementAtIndex(j);

                        var currentApplyCount = applyActionProperty.FindPropertyRelative("currentApplyCount");
                        var needSelectionResultTypeProperty = applyActionProperty.FindPropertyRelative("needSelectionResultType");
                        var applyTypeProperty = applyActionProperty.FindPropertyRelative("applyType");
                        var currentPrecedingAction = applyActionProperty.FindPropertyRelative("precedingAction");
                        var currentAction = applyActionProperty.FindPropertyRelative("action");
                        var currentTargetSearcher = applyActionProperty.FindPropertyRelative("targetSearcher");
                        var currentEffect = applyActionProperty.FindPropertyRelative("effectSelectors");
                        var currentSkillFinishOption = applyActionProperty.FindPropertyRelative("inSkillActionFinishOption");
                        var currentPrecedingActionAnimatorParameter 
                            = applyActionProperty.FindPropertyRelative("precedingActionAnimatorParameter");
                        var currentActionAnimatorParameter = applyActionProperty.FindPropertyRelative("actionAnimatorParameter");
                        var currentCustomActionsOnPrecedingAction = applyActionProperty.FindPropertyRelative("customActionsOnPrecedingAction");
                        var currentCustomActionsOnAction = applyActionProperty.FindPropertyRelative("customActionsOnAction");

                        if (DrawFoldoutTitle("Apply Action " + (j + 1)))
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Apply Action", EditorStyles.boldLabel);

                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(currentApplyCount);
                            GUI.enabled = true;

                            CustomEditorUtility.DrawEnumToolbar(needSelectionResultTypeProperty);

                            if (lsDrawPropertyAll)
                                CustomEditorUtility.DrawEnumToolbar(applyTypeProperty);
                            //  Passive or Toggle 타입이라면 applyType 값을 0으로 고정 
                            // 2) applyType     : Instant
                            else
                                applyTypeProperty.enumValueIndex = 0;

                            // Toggle Type일 때는 PrecedingAction을 사용하지 않을 것이므로, Instant Type일 때만 PrecedingAction 변수를 보여줌
                            if (useTypeProperty.enumValueIndex == (int)SkillUseType.Instant)
                                EditorGUILayout.PropertyField(currentPrecedingAction);

                            EditorGUILayout.PropertyField(currentAction);
                            EditorGUILayout.PropertyField(currentTargetSearcher);

                            // Effect 작성 
                            EditorGUILayout.PropertyField(currentEffect);

                            // EffectSelector의 level 변수를 effect의 최대 level로 제한함
                            for (int k = 0; k < currentEffect.arraySize; k++)
                            {
                                var effectSelectorProperty = currentEffect.GetArrayElementAtIndex(k);
                                // Selector의 level Property를 가져옴
                                var levelProperty = effectSelectorProperty.FindPropertyRelative("level");
                                // Selector가 가진 effect를 가져옴
                                // → FindPropertyRelative는 SerializedProperty로 반환하기 때문에 objectReferenceValue로 가져와 as로 캐스팅한다.
                                var effect = effectSelectorProperty.FindPropertyRelative("effect").objectReferenceValue as Effect;
                                // maxLevel은 effect가 null이 아니면 effect의 최대 Level, null이면 0으로 설정
                                var maxLevel = effect != null ? effect.MaxLevel : 0;
                                // minLevel은 maxLevel이 0이면 0, 아니면 1로 설정 
                                var minLevel = maxLevel == 0 ? 0 : 1;
                                // EffectSelector의 level 변수 값을 위에서 구한 minLevel, maxLevel로 Clamping 해준다.
                                levelProperty.intValue = Mathf.Clamp(levelProperty.intValue, minLevel, maxLevel);
                            }

                            EditorGUILayout.PropertyField(currentSkillFinishOption);
                            EditorGUILayout.PropertyField(currentPrecedingActionAnimatorParameter);
                            EditorGUILayout.PropertyField(currentActionAnimatorParameter);

                            // Custom Action - UnderlineTitle
                            EditorGUILayout.Space();
                            // Custom Action이라는 Text를 Bold체로 그려준다. 
                            EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
                            // 구분선 
                            CustomEditorUtility.DrawUnderLine();

                            var currentCustomActionToolbarIndex = customActionToolbarIndexesByApplyCount.ContainsKey(j)
                                ? customActionToolbarIndexesByApplyCount[j]
                                : 0;

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(12);

                                currentCustomActionToolbarIndex
                                    = GUILayout.Toolbar(currentCustomActionToolbarIndex, currentCustomActionsToolbarList);

                                customActionToolbarIndexesByApplyCount[j] = currentCustomActionToolbarIndex;
                            }
                            GUILayout.EndHorizontal();

                            if (currentCustomActionToolbarIndex == 0)
                                EditorGUILayout.PropertyField(currentCustomActionsOnPrecedingAction);
                            else
                                EditorGUILayout.PropertyField(currentCustomActionsOnAction);
                        }
                    }

                    EditorGUI.indentLevel -= 1;

                    EditorGUILayout.Space();
                    CustomEditorUtility.DrawUnderLine();
                    EditorGUILayout.Space();

                    // Cooldown Property 그려주기 
                    property.NextVisible(false);
                    EditorGUILayout.PropertyField(property);

                    // Cast : isUseCast 변수
                    property.NextVisible(false);
                    // IsDrawPropertyAll이 true이고 isUseCharge가 false면 cast 변수를 그려준다. 
                    // → isUseCast는 Skill이 Passive Type이거나 Toggle Type이거나 Charge를 사용한다면 그려주지 않는다. 
                    if (lsDrawPropertyAll && !isUseChargeProperty.boolValue)
                        EditorGUILayout.PropertyField(property);
                    else
                        property.boolValue = false;

                    // castTime 변수 
                    property.NextVisible(false);
                    // isUsecast가 true라면 변수를 그려준다. 
                    if (isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    // Charge : isUseCharge 변수 
                    property.NextVisible(false);
                    // IsDrawPropertyAll이 true이고 isUseCast가 false면 charge 변수를 그려준다.
                    if (lsDrawPropertyAll && !isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    // Charge와 관련된 변수 7개를 그려준다. 
                    for (int j = 0; j < 7; j++)
                    {
                        property.NextVisible(false);
                        if (isUseChargeProperty.boolValue)
                            EditorGUILayout.PropertyField(property);
                    }

                    // 최대 chargeTime 값을 chargeDuration 값으로 제한
                    // → Full Charge에 걸리는 시간은 Charge의 지속 시간을 넘을 수 없다. 
                    chargeTimeProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, chargeDurationProperty.floatValue);

                    // 최대 needChargeTime 값을 chargeTime 값으로 제한
                    // → 사용에 필요한 최소 Charge 시간은 Full Charge 시간을 넘을 수 없다. 
                    needChargeTimeToUseProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, needChargeTimeToUseProperty.floatValue);

                    // Animation 변수들을 그려준다. 
                    // 1) castAnimatorParameter
                    // 2) chargeAnimatorParameter
                    for (int j = 0; j < 2; j++)
                    {
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    // Custom Action - UnderlineTitle
                    EditorGUILayout.Space();
                    // Custom Action이라는 Text를 Bold체로 그려준다. 
                    EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
                    // 구분선 
                    CustomEditorUtility.DrawUnderLine();

                    // Custom Action - Toolbar
                    // 한번에 모든 Array 변수를 다 그리면 보기 번잡하니 Toolbar를 통해 보여줄 Array를 선택할 수 있게 한다.
                    // ※ customActionToolbarIndex : 현재 Data에서 어떤 Toolbar 항목이 선택되었는지 나타내는 변수
                    // → customActionToolbarIndexesByLevel Dictionary 변수에 현재 level의 SkillData Index(0 ~ 3, Toolbar 번호)가 존재하는지 확인해서 
                    //    있다면 현재 Data에서 선택되어 있는 Toolbar 항목 Index를 가져오고 없다면 배열의 첫 번째 Index인 0(Cast)을 가져온다. 
                    var customActionToolbarIndex = customActionToolbarIndexesByLevel.ContainsKey(i) ? customActionToolbarIndexesByLevel[i] : 0;

                    // Toolbar는 자동 들여쓰기(EditorGUI.indentLevel)가 먹히지 않아서 직접 들여쓰기를 해줌
                    GUILayout.BeginHorizontal();
                    {
                        // 수평 정렬이 실행 중이기 때문에 그냥 Space처럼 위를 띄우는게 아니라 왼쪽을 띄우게 된다. 
                        GUILayout.Space(12);

                        // ※ GUILayout.Toolbar(int selected, string[] texts)
                        // 1) selected : 선택된 항목의 index
                        // 2) texts    : 항목들의 이름 Ex) Cast, Charge ... 
                        // return      : 선택한 이름의 Index
                        customActionToolbarIndex = GUILayout.Toolbar(customActionToolbarIndex, customActionsToolbarList);
                        // return된 Index 값을 Dictionary에 저장해서 Skill을 보고 있는 동안은 선택이 계속 유지되도록 해 준다. 
                        customActionToolbarIndexesByLevel[i] = customActionToolbarIndex;
                    }
                    GUILayout.EndHorizontal();

                    // Custom Action
                    for (int j = 0; j < 2; j++)
                    {
                        property.NextVisible(false);
                        // for문 Index 값이 위에서 선택된 Toolbar Index와 같다면 그려준다. 
                        // → Toolbar에서 선택된 Custom Action만 보여준다. 
                        if (j == customActionToolbarIndex)
                            EditorGUILayout.PropertyField(property);
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
            // Help Box 끝
            EditorGUILayout.EndVertical();

        } // 해당 과정이 Data 수만큼 반복되며 모든 Data를 그리게 된다. 

        #region Iterator 방식 단점 
        // → 코드 작성하기에는 편한데 다른 사람이 보기가 힘들다는 단점이 있다. 
        //    다만, for문을 돌면서 변수를 그릴 수 있다는 점은 분명한 장점이기 때문에 필요에 따라서 제한된 곳에 쓰면 된다. 
        #endregion

        // 새로운 SkillData를 추가
        if (GUILayout.Button("Add New Level"))
        {
            // Level Change
            var lastArraySize = skillDatasProperty.arraySize++;
            var prevElementalProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            var newElementProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize);

            // 새로운 Data의 level을 이전 Data의 level + 1로 설정한다.
            var newElementLevel = prevElementalProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;
            newElementProperty.isExpanded = true;

            var newApplyActionsProperty = newElementProperty.FindPropertyRelative("applyActions");

            for (int i = 0; i < newApplyActionsProperty.arraySize; i++)
            {
                CustomEditorUtility.DeepCopySerializeReference(newApplyActionsProperty.GetArrayElementAtIndex(i).
                    FindPropertyRelative("precedingAction"));
                CustomEditorUtility.DeepCopySerializeReference(newApplyActionsProperty.GetArrayElementAtIndex(i).
                    FindPropertyRelative("action"));

                // TargetSearcher SelectionAction & SearchAction Deep Copy
                CustomEditorUtility.DeepCopySerializeReference(
                    newApplyActionsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("targetSearcher").
                    FindPropertyRelative("selectionAction"));
                CustomEditorUtility.DeepCopySerializeReference(
                    newApplyActionsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("targetSearcher").
                    FindPropertyRelative("searchAction"));

                CustomEditorUtility.DeepCopySerializeReferenceArray(newApplyActionsProperty.GetArrayElementAtIndex(i).
                    FindPropertyRelative("customActionsOnPrecedingAction"));
                CustomEditorUtility.DeepCopySerializeReferenceArray(newApplyActionsProperty.GetArrayElementAtIndex(i).
                    FindPropertyRelative("customActionsOnAction"));
            }

            // Custom Actions Deep Copy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnCast"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnCharge"));
        }
    }
}
