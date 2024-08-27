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
        // R키를 누르면 targetSearcher의 ShowIndicator 함수를 실행하고 SelectTarget 함수로 기준점 검색을 실행한다. 
        else if (Input.GetKeyDown(KeyCode.R))
        {
            targetSearcher.Scale = scale;
            targetSearcher.ShowIndicator(gameObject);
            // → SelectionCompletedHandler의 인자값인 targetSearcher는 생략 : _ 
            targetSearcher.SelectTarget(GetComponent<Entity>(), gameObject, (_, selectionResult) =>
            {
                // ※ 코드 작동 순서 
                // 1. Select 함수로 기준점 검색 완료 후
                // 2. SelectCompletedHandler event 실행 
                // 3. SelectCompletedHandler event에 등록된 TargetSearcher.OnSelectCompleted 함수 실행 
                // 4. onSelectionCompleted?.Invoke(this, selectReuslt); 코드 실행하면서 아래 작성된 코드가 실행 

                targetSearcher.HideIndicator();
                switch (selectionResult.resultMessage)
                {
                    // 기준점 검색 실패
                    case SearchResultMessage.Fail:
                        Debug.Log("<color=red>Select Failed.</color>");
                        break;

                    case SearchResultMessage.OutOfRange:
                        Debug.Log("<color=yellow>Out Of Range</color>");
                        break;

                    // 기준점 검색 성공 
                    default:
                        // 기준점 : Target
                        if (selectionResult.selectedTarget)
                            Debug.Log($"<color=green>Selected Target: {selectionResult.selectedTarget.name}</color>");
                        // 기준점 : Position 
                        else
                            Debug.Log($"<color=green>Selected Position: {selectionResult.selectedPosition}</color>");

                        // 기준점 검색 성공 → SearchTargets 함수로 실제 Traget들을 찾아옴
                        var searchResult = targetSearcher.SearchTargets(GetComponent<Entity>(), gameObject);
                        // Targets인 경우 
                        if (searchResult.targets.Count > 0)
                        {
                            foreach (var target in searchResult.targets)
                                Debug.Log($"<color=#FF00FF>Searched Target: {target.name}</color>");
                        }
                        // Positions인 경우
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
