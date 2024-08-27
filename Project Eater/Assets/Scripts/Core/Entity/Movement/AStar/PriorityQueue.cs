using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ 추가사항
// → 내림차순으로 정렬하는 것이 아니라 오름차순으로 하고 싶으면 
//    push하는 데이터의 value에 -1을 곱해서 push하고 나중에 해당 값을 쓸 때, 다시 -1을 곱해서 사용하면 된다. 
// → 현재 코드는 CompareTo의 <, > 를 바꾼 상태! 
public class PriorityQueue<T> where T : IComparable<T> // 대소 비교를 할 수 있는 데이터형만 T에 들어올 수 있다. 
{
    // compare(variant) will be < 0 if this instance value is less than other.value
    // compare(variant) will be > 0 if this instance value is greater than other.value
    // compare(variant) will be == 0 if the values are the same

    // 힙은 동적 배열로 생성
    // → 최소 힙
    List<T> heap = new List<T>();

    public void Push(T node)
    {
        // 힙의 맨 끝에 새로운 데이터를 삽입한다. 
        heap.Add(node);

        // 새로운 데이터가 들어간 인덱스 가져오기 
        int comparisonIndex = heap.Count - 1;

        // 부모, 자식 노드 값 비교 
        // → 부모 노드는 자식 노드보다 값이 클 수 없다. 
        while (comparisonIndex > 0) // root에 도달하면 멈춤
        {
            // 부모 인덱스 가져오기 
            int parentIndex = (comparisonIndex - 1) / 2;
            if (heap[comparisonIndex].CompareTo(heap[parentIndex]) > 0)
                break; // 부모 값이 더 작기 때문에 멈추기

            // 아니면 두 값을 교체한다. 
            T temp = heap[comparisonIndex];
            heap[comparisonIndex] = heap[parentIndex];
            heap[parentIndex] = temp;

            // 검사 위치를 갱신
            comparisonIndex = parentIndex;
        }
    }
    
    // 가장 작은 값을 Pop : root 값
    public T Pop()
    {
        // 반환할 데이터 따로 저장
        T retData = heap[0];

        // ※ 재배치 과정 
        // 마지막 데이터를 root로 이동
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];

        // 마지막 데이터(인덱스) 제거
        heap.RemoveAt(lastIndex);
        lastIndex--;

        // 자식, 부모 노드 값 비교 
        int comparisonIndex = 0;
        while (true)
        {
            int leftIndex = 2 * comparisonIndex + 1;
            int rightIndex = 2 * comparisonIndex + 2;

            int nextIndex = comparisonIndex;

            // 공통 : 자식 노드의 인덱스 값이 마지막 인덱스를 넘어가지 않아야 한다. 
            // → 왼쪽 값이 현재값보다 작으면, 왼쪽으로 이동
            if (leftIndex <= lastIndex && heap[nextIndex].CompareTo(heap[leftIndex]) > 0)
                nextIndex = leftIndex;
            // → 오른쪽 값이 현재값(왼쪽으로 이동한 값 포함)보다 작으면, 오른쪽으로 이동
            // ( 이를 통해, 양쪽 모두 작을 경우, 더 작은 쪽이랑 교체할 수 있게 된다. )
            if (rightIndex <= lastIndex && heap[nextIndex].CompareTo(heap[rightIndex]) > 0)
                nextIndex = rightIndex;

            // 자식 노드의 값(왼쪽/오른쪽 자식 노드) 모두 현재 값보다 크면 종료
            if (nextIndex == comparisonIndex)
                break;

            // 아니면 두 값을 교체한다. 
            T temp = heap[comparisonIndex];
            heap[comparisonIndex] = heap[nextIndex];
            heap[nextIndex] = temp;

            // 검사 위치를 이동한다. 
            comparisonIndex = nextIndex;
        }

        return retData;
    }

    public int Count()
    {
        return heap.Count;
    }

    public bool Contains(T node)
    {
        return heap.Contains(node);
    }
}
