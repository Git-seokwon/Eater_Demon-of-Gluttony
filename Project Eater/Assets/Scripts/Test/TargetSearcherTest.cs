using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetSearcherTest : MonoBehaviour
{
    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private TargetSearcher targetSearcher;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            scale -= 0.1f;
            targetSearcher.Scale = scale;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            scale += 0.1f;
            targetSearcher.Scale = scale;
        }
        // RŰ�� ������ targetSearcher�� ShowIndicator �Լ��� �����ϰ� SelectTarget �Լ��� ������ �˻��� �����Ѵ�. 
        else if (Input.GetKeyDown(KeyCode.R))
        {
            targetSearcher.Scale = scale;
            targetSearcher.ShowIndicator(gameObject);
            // �� SelectionCompletedHandler�� ���ڰ��� targetSearcher�� ���� : _ 
            targetSearcher.SelectTarget(GetComponent<Entity>(), gameObject, (_, selectionResult) =>
            {
                // �� �ڵ� �۵� ���� 
                // 1. Select �Լ��� ������ �˻� �Ϸ� ��
                // 2. SelectCompletedHandler event ���� 
                // 3. SelectCompletedHandler event�� ��ϵ� TargetSearcher.OnSelectCompleted �Լ� ���� 
                // 4. onSelectionCompleted?.Invoke(this, selectReuslt); �ڵ� �����ϸ鼭 �Ʒ� �ۼ��� �ڵ尡 ���� 

                targetSearcher.HideIndicator();
                switch (selectionResult.resultMessage)
                {
                    // ������ �˻� ����
                    case SearchResultMessage.Fail:
                        Debug.Log("<color=red>Select Failed.</color>");
                        break;

                    case SearchResultMessage.OutOfRange:
                        Debug.Log("<color=yellow>Out Of Range</color>");
                        break;

                    // ������ �˻� ���� 
                    default:
                        // ������ : Target
                        if (selectionResult.selectedTarget)
                            Debug.Log($"<color=green>Selected Target: {selectionResult.selectedTarget.name}</color>");
                        // ������ : Position 
                        else
                            Debug.Log($"<color=green>Selected Position: {selectionResult.selectedPosition}</color>");

                        // ������ �˻� ���� �� SearchTargets �Լ��� ���� Traget���� ã�ƿ�
                        var searchResult = targetSearcher.SearchTargets(GetComponent<Entity>(), gameObject);
                        // Targets�� ��� 
                        if (searchResult.targets.Count > 0)
                        {
                            foreach (var target in searchResult.targets)
                                Debug.Log($"<color=#FF00FF>Searched Target: {target.name}</color>");
                        }
                        // Positions�� ���
                        else
                        {
                            foreach (var position in searchResult.positions)
                                Debug.Log($"<color=#FF00FF>Searched Position: {position}</color>");
                        }
                        break;
                }
            });
        }
    }
}
