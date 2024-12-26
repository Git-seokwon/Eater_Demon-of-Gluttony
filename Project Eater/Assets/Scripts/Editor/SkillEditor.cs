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

    // Toolbar Button���� �̸� : "Cast", "Charge", "Preceding", "Action"
    // 1) Cast   : customActionsOnCast ������ �׷��� 
    // 2) Charge : customActionsOnCharge ������ �׷��� 
    // ... 
    // �� �� ���¿��� CustomActions�� �׸� �� ���� (������)
    private readonly string[] customActionsToolbarList = new[] { "Cast", "Charge" };
    private readonly string[] currentCustomActionsToolbarList = new[] { "Preceding", "Action" };

    // Skill Data���� ������ Toolbar Button�� Index ��
    // �� Level Data���� customActions�� �����ϴµ� 1 Level������ Cast, 2 Level������ Charge�� ������ ���� �ִ�. 
    //    �̷��� Data���� �� ��° Toolbar Button�� �����ߴ��� ����ϴ� ���� 
    // �� Key   : Data Level
    // �� Value : ���õ� Button�� Index
    private Dictionary<int, int> customActionToolbarIndexesByLevel = new();

    // �� Key   : Apply Count Action
    // �� Value : ���õ� Button�� Index
    private Dictionary<int, int> customActionToolbarIndexesByApplyCount = new();

    private bool IsPassive => typeProperty.enumValueIndex == (int)SkillType.Passive;
    private bool IsToggleType => useTypeProperty.enumValueIndex == (int)SkillUseType.Toggle;

    // Toggle, Passive Type�� ���� ������� �ʴ� �������� �������� ���� ����
    // �� IsToggleType, IsPassive ��� false�� ��쿡�� IsDrawPropertyAll�� true�� �Ǿ� ��� Property���� �׷��� ��
    private bool lsDrawPropertyAll => !IsToggleType && !IsPassive;

    protected override void OnEnable()
    {
        base.OnEnable();

        // FindProperty�� Skill�� �������� �����´�. 
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
        // Label�� ���̸� �������� �� �߸��� �ʰ� ���̵��� 220���� �ø���.
        EditorGUIUtility.labelWidth = 260f;

        // �� �׸���� �׷��ش�. 
        DrawSettings();
        DrawUseConditions();
        DrawSkillDatas();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    // �� DrawSettings : Skill�� Enum �������� �׸��� �Լ� 
    private void DrawSettings()
    {
        // Setting foldout ��ư�� �׸��� �������� �ʾ����� return 
        if (!DrawFoldoutTitle("Setting"))
            return;

        // �� typeProperty : Passive or Active
        CustomEditorUtility.DrawEnumToolbar(typeProperty);
        // �� gradeProperty : Common, Rare, Unique
        CustomEditorUtility.DrawEnumToolbar(gradeProperty);
        // �� movementProperty : Move, Stop
        CustomEditorUtility.DrawEnumToolbar(movementProperty);
        EditorGUILayout.PropertyField(stackCountDisplayroperty);

        if (!IsPassive)
            // Active ��ų�̸� Instant or Toggle Enum Button�� �׸� 
            CustomEditorUtility.DrawEnumToolbar(useTypeProperty);
        else
            // instant�� ����
            // �� Passive ��ų�� ������ Instant ��ų�� ���� 
            // �� Passive ��ų�� Toggle Type�� �� �� ����.
            useTypeProperty.enumValueIndex = 0;

        // Passive or Toggle Ÿ���� �ƴ϶�� 
        if (lsDrawPropertyAll)
        {
            EditorGUILayout.Space();
            CustomEditorUtility.DrawUnderLine();
            EditorGUILayout.Space();

            // executionType�� applyType�� �׷��ش�. 
            CustomEditorUtility.DrawEnumToolbar(executionTypeProperty);
        }
        //  Passive or Toggle Ÿ���̶�� executionType ���� 0���� ���� 
        // 1) executionType : Auto
        else
            executionTypeProperty.enumValueIndex = 0;

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderLine();
        EditorGUILayout.Space();

        CustomEditorUtility.DrawEnumToolbar(targetSelectionTimingOptionProperty);
        CustomEditorUtility.DrawEnumToolbar(targetSearchTimingOptionProperty);
    }

    // �� DrawUseConditions : UseConditions Property �׷��ִ� �Լ� 
    private void DrawUseConditions()
    {
        // DrawFoldoutTitle Title�� �������� ������ return  
        if (!DrawFoldoutTitle("Use Condition"))
            return;

        EditorGUILayout.PropertyField(useConditionsProperty);
    }

    private void DrawSkillDatas()
    {
        // Skill�� Data�� �ƹ��͵� �������� ������(Data�� ���̰� 0�̶��) 1��(1 Level Data)�� �ڵ������� �������
        if (skillDatasProperty.arraySize == 0)
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            skillDatasProperty.arraySize++;
            // �߰��� Data�� Level(SkillData[0].level)�� 1�� ����
            skillDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        // Data Title�� �������� ������ return 
        if (!DrawFoldoutTitle("Data"))
            return;

        // �� MaxLevel�� level �������� ���� ��Ű�� �۾�
        // Property�� �������� ���ϰ� GUI Enable�� false�� �ٲ�
        GUI.enabled = false;
        var lastIndex = skillDatasProperty.arraySize - 1;
        // ������ SkillData(= ���� ���� Level�� Data)�� ������
        var lastSkillData = skillDatasProperty.GetArrayElementAtIndex(lastIndex);
        // maxLevel�� ������ Data�� Level�� ����
        maxLevelProperty.intValue = lastSkillData.FindPropertyRelative("level").intValue;
        // maxLevel Property�� �׷���
        EditorGUILayout.PropertyField(maxLevelProperty);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(defaultLevelProperty);

        // SkillDatas ��ȸ
        for (int i = 0; i < skillDatasProperty.arraySize; i++)
        {
            // ���� Index�� SkillData �������� 
            var property = skillDatasProperty.GetArrayElementAtIndex(i);

            // applyActions�� ���� �۾��� �ʿ��ϱ� ������ ������ 
            var skillApplyActionProperty = property.FindPropertyRelative("applyActions");
            var applyCount = property.FindPropertyRelative("applyCount");
            // isUseCast ~ needChargeTimeToUse�� ���� �۾��� �ʿ��ϱ� ������ ������ ������
            var isUseCastProperty = property.FindPropertyRelative("isUseCast");
            var isUseChargeProperty = property.FindPropertyRelative("isUseCharge");
            var chargeDurationProperty = property.FindPropertyRelative("chargeDuration");
            var chargeTimeProperty = property.FindPropertyRelative("chargeTime");
            var needChargeTimeToUseProperty = property.FindPropertyRelative("needChargeTimeToUse");

            // HelpBox �׷��ֱ� 
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
                // ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ���� (i != 0)
                // X Button�� ������ Data�� �������� true�� return ��
                if (DrawRemovableLevelFoldout(skillDatasProperty, property, i, i != 0))
                {
                    // Data�� �����Ǿ����� �� �̻� GUI�� �׸��� �ʰ� �ٷ� ��������
                    // ���� Frame�� ó������ �ٽ� �׸��� ����
                    EditorGUILayout.EndVertical();
                    break;
                }

                // �鿩���� 1ĭ 
                // �� Foldout â�� level ������ ���� ���ο��� �׷������� �ϱ� ���� 
                EditorGUI.indentLevel += 1;

                if (property.isExpanded)
                {
                    #region SerializedProperty�� Iterator
                    // �� SerializedProperty�� �ܼ��� ������ �ƴ϶� �ݺ��ڶ�� �θ��� Iterator�̴�. 
                    //    Iterator�� ���� ���� ���� ���� ��ȸ�� �� �ְ� ���ִ� ��ü�̴�. 
                    //    ���⼭ ���� ���̶�� �� ã�ƿ� ������ �����ؼ� ������ ��� Serialize ������ �ǹ��Ѵ�. 
                    // Ex) FindProperty�� type ������ ã�� ���� ��, �� ���� �ϳ��� �ٸ� ��� �������� ������ �� �� �ִ�. 

                    // �� Next, NextVisible �Լ� 
                    // �� �ٸ� ������ �����ϴ� ����� �ٷ�, SerialzeProperty�� Next Ȥ�� NextVisible �Լ��̴�. 
                    // Ex) type Property�� NextVisible �Լ��� ����ϰ� �Ǹ� type Property�� ���� type ������ �ٷ� �Ʒ� ����� 
                    //     ���� ������ useType ������ �ȴ�. �� ���¿��� PropertyField �Լ��� typeProperty�� �׷��ְ� �Ǹ�
                    //     useType�� �׷����� �ȴ�. ��, NextVisible �Լ��� �������� SerializeProperty�� ���� �� ���� ������ �Ѿ�� �ȴ�. 

                    // �� Next �Լ��� NextVisible �Լ��� ������
                    // NextVisible �Լ��� HideInInspector Attribute�� �޷��ִ� Serialize ������ �ǳʶڴ�. 

                    // �� NextVisible �Լ� 
                    // 1) ��ȯ�� bool : �� �̻� �̵��� ������ ������ false�� ��ȯ�Ѵ�. 
                    // �� while ���� �̿��Ͽ� NextVisible �Լ��� false�� ��ȯ�� ������ �̵��ϸ鼭 PropertyField �Լ��� ������ �׸��� ����� ����
                    // 2) ���ڰ� bool : ���� ���� Type�� Class�� Struct��� �� ���� ���η� �� �� ����  
                    // Ex) property(Skill Data)�� ���� Serialize ����(Level)�� �̵�
                    #endregion

                    // SkillData Property ���η� �� -> Property�� SkillData�� level field�� �ȴ�.
                    property.NextVisible(true);

                    // Level Property�� �׷��ֱ�
                    // �� Level ������ ������ �� ����. 
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(property);
                    GUI.enabled = true;

                    // SkillRunningFinishOption �׷��ֱ� 
                    property.NextVisible(true);
                    CustomEditorUtility.DrawEnumToolbar(property);

                    // 1) duration
                    // 2) applyCount
                    // 3) applyCycle
                    for (int j = 0; j < 3; j++)
                    {
                        // ���� ������ Property�� �̵��ϸ鼭 �׷���
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

                    // Skill Apply Action ��ȸ 
                    for (int j = 0; j < skillApplyActionProperty.arraySize; j++)
                    {
                        // ���� Apply Count�� Skill Apply Action �������� 
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
                            //  Passive or Toggle Ÿ���̶�� applyType ���� 0���� ���� 
                            // 2) applyType     : Instant
                            else
                                applyTypeProperty.enumValueIndex = 0;

                            // Toggle Type�� ���� PrecedingAction�� ������� ���� ���̹Ƿ�, Instant Type�� ���� PrecedingAction ������ ������
                            if (useTypeProperty.enumValueIndex == (int)SkillUseType.Instant)
                                EditorGUILayout.PropertyField(currentPrecedingAction);

                            EditorGUILayout.PropertyField(currentAction);
                            EditorGUILayout.PropertyField(currentTargetSearcher);

                            // Effect �ۼ� 
                            EditorGUILayout.PropertyField(currentEffect);

                            // EffectSelector�� level ������ effect�� �ִ� level�� ������
                            for (int k = 0; k < currentEffect.arraySize; k++)
                            {
                                var effectSelectorProperty = currentEffect.GetArrayElementAtIndex(k);
                                // Selector�� level Property�� ������
                                var levelProperty = effectSelectorProperty.FindPropertyRelative("level");
                                // Selector�� ���� effect�� ������
                                // �� FindPropertyRelative�� SerializedProperty�� ��ȯ�ϱ� ������ objectReferenceValue�� ������ as�� ĳ�����Ѵ�.
                                var effect = effectSelectorProperty.FindPropertyRelative("effect").objectReferenceValue as Effect;
                                // maxLevel�� effect�� null�� �ƴϸ� effect�� �ִ� Level, null�̸� 0���� ����
                                var maxLevel = effect != null ? effect.MaxLevel : 0;
                                // minLevel�� maxLevel�� 0�̸� 0, �ƴϸ� 1�� ���� 
                                var minLevel = maxLevel == 0 ? 0 : 1;
                                // EffectSelector�� level ���� ���� ������ ���� minLevel, maxLevel�� Clamping ���ش�.
                                levelProperty.intValue = Mathf.Clamp(levelProperty.intValue, minLevel, maxLevel);
                            }

                            EditorGUILayout.PropertyField(currentSkillFinishOption);
                            EditorGUILayout.PropertyField(currentPrecedingActionAnimatorParameter);
                            EditorGUILayout.PropertyField(currentActionAnimatorParameter);

                            // Custom Action - UnderlineTitle
                            EditorGUILayout.Space();
                            // Custom Action�̶�� Text�� Boldü�� �׷��ش�. 
                            EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
                            // ���м� 
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

                    // Cooldown Property �׷��ֱ� 
                    property.NextVisible(false);
                    EditorGUILayout.PropertyField(property);

                    // Cast : isUseCast ����
                    property.NextVisible(false);
                    // IsDrawPropertyAll�� true�̰� isUseCharge�� false�� cast ������ �׷��ش�. 
                    // �� isUseCast�� Skill�� Passive Type�̰ų� Toggle Type�̰ų� Charge�� ����Ѵٸ� �׷����� �ʴ´�. 
                    if (lsDrawPropertyAll && !isUseChargeProperty.boolValue)
                        EditorGUILayout.PropertyField(property);
                    else
                        property.boolValue = false;

                    // castTime ���� 
                    property.NextVisible(false);
                    // isUsecast�� true��� ������ �׷��ش�. 
                    if (isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    // Charge : isUseCharge ���� 
                    property.NextVisible(false);
                    // IsDrawPropertyAll�� true�̰� isUseCast�� false�� charge ������ �׷��ش�.
                    if (lsDrawPropertyAll && !isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    // Charge�� ���õ� ���� 7���� �׷��ش�. 
                    for (int j = 0; j < 7; j++)
                    {
                        property.NextVisible(false);
                        if (isUseChargeProperty.boolValue)
                            EditorGUILayout.PropertyField(property);
                    }

                    // �ִ� chargeTime ���� chargeDuration ������ ����
                    // �� Full Charge�� �ɸ��� �ð��� Charge�� ���� �ð��� ���� �� ����. 
                    chargeTimeProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, chargeDurationProperty.floatValue);

                    // �ִ� needChargeTime ���� chargeTime ������ ����
                    // �� ��뿡 �ʿ��� �ּ� Charge �ð��� Full Charge �ð��� ���� �� ����. 
                    needChargeTimeToUseProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, needChargeTimeToUseProperty.floatValue);

                    // Animation �������� �׷��ش�. 
                    // 1) castAnimatorParameter
                    // 2) chargeAnimatorParameter
                    for (int j = 0; j < 2; j++)
                    {
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    // Custom Action - UnderlineTitle
                    EditorGUILayout.Space();
                    // Custom Action�̶�� Text�� Boldü�� �׷��ش�. 
                    EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
                    // ���м� 
                    CustomEditorUtility.DrawUnderLine();

                    // Custom Action - Toolbar
                    // �ѹ��� ��� Array ������ �� �׸��� ���� �����ϴ� Toolbar�� ���� ������ Array�� ������ �� �ְ� �Ѵ�.
                    // �� customActionToolbarIndex : ���� Data���� � Toolbar �׸��� ���õǾ����� ��Ÿ���� ����
                    // �� customActionToolbarIndexesByLevel Dictionary ������ ���� level�� SkillData Index(0 ~ 3, Toolbar ��ȣ)�� �����ϴ��� Ȯ���ؼ� 
                    //    �ִٸ� ���� Data���� ���õǾ� �ִ� Toolbar �׸� Index�� �������� ���ٸ� �迭�� ù ��° Index�� 0(Cast)�� �����´�. 
                    var customActionToolbarIndex = customActionToolbarIndexesByLevel.ContainsKey(i) ? customActionToolbarIndexesByLevel[i] : 0;

                    // Toolbar�� �ڵ� �鿩����(EditorGUI.indentLevel)�� ������ �ʾƼ� ���� �鿩���⸦ ����
                    GUILayout.BeginHorizontal();
                    {
                        // ���� ������ ���� ���̱� ������ �׳� Spaceó�� ���� ���°� �ƴ϶� ������ ���� �ȴ�. 
                        GUILayout.Space(12);

                        // �� GUILayout.Toolbar(int selected, string[] texts)
                        // 1) selected : ���õ� �׸��� index
                        // 2) texts    : �׸���� �̸� Ex) Cast, Charge ... 
                        // return      : ������ �̸��� Index
                        customActionToolbarIndex = GUILayout.Toolbar(customActionToolbarIndex, customActionsToolbarList);
                        // return�� Index ���� Dictionary�� �����ؼ� Skill�� ���� �ִ� ������ ������ ��� �����ǵ��� �� �ش�. 
                        customActionToolbarIndexesByLevel[i] = customActionToolbarIndex;
                    }
                    GUILayout.EndHorizontal();

                    // Custom Action
                    for (int j = 0; j < 2; j++)
                    {
                        property.NextVisible(false);
                        // for�� Index ���� ������ ���õ� Toolbar Index�� ���ٸ� �׷��ش�. 
                        // �� Toolbar���� ���õ� Custom Action�� �����ش�. 
                        if (j == customActionToolbarIndex)
                            EditorGUILayout.PropertyField(property);
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
            // Help Box ��
            EditorGUILayout.EndVertical();

        } // �ش� ������ Data ����ŭ �ݺ��Ǹ� ��� Data�� �׸��� �ȴ�. 

        #region Iterator ��� ���� 
        // �� �ڵ� �ۼ��ϱ⿡�� ���ѵ� �ٸ� ����� ���Ⱑ ����ٴ� ������ �ִ�. 
        //    �ٸ�, for���� ���鼭 ������ �׸� �� �ִٴ� ���� �и��� �����̱� ������ �ʿ信 ���� ���ѵ� ���� ���� �ȴ�. 
        #endregion

        // ���ο� SkillData�� �߰�
        if (GUILayout.Button("Add New Level"))
        {
            // Level Change
            var lastArraySize = skillDatasProperty.arraySize++;
            var prevElementalProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            var newElementProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize);

            // ���ο� Data�� level�� ���� Data�� level + 1�� �����Ѵ�.
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
