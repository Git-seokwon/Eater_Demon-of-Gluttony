using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �߰�����
// �� ������������ �����ϴ� ���� �ƴ϶� ������������ �ϰ� ������ 
//    push�ϴ� �������� value�� -1�� ���ؼ� push�ϰ� ���߿� �ش� ���� �� ��, �ٽ� -1�� ���ؼ� ����ϸ� �ȴ�. 
// �� ���� �ڵ�� CompareTo�� <, > �� �ٲ� ����! 
public class PriorityQueue<T> where T : IComparable<T> // ��� �񱳸� �� �� �ִ� ���������� T�� ���� �� �ִ�. 
{
    // compare(variant) will be < 0 if this instance value is less than other.value
    // compare(variant) will be > 0 if this instance value is greater than other.value
    // compare(variant) will be == 0 if the values are the same

    // ���� ���� �迭�� ����
    // �� �ּ� ��
    List<T> heap = new List<T>();

    private HashSet<T> openNodeList = new HashSet<T>();

    public void Push(T node)
    {
        // ���� �� ���� ���ο� �����͸� �����Ѵ�. 
        heap.Add(node);
        openNodeList.Add(node);

        // ���ο� �����Ͱ� �� �ε��� �������� 
        int comparisonIndex = heap.Count - 1;

        // �θ�, �ڽ� ��� �� �� 
        // �� �θ� ���� �ڽ� ��庸�� ���� Ŭ �� ����. 
        while (comparisonIndex > 0) // root�� �����ϸ� ����
        {
            // �θ� �ε��� �������� 
            int parentIndex = (comparisonIndex - 1) / 2;
            if (heap[comparisonIndex].CompareTo(heap[parentIndex]) > 0)
                break; // �θ� ���� �� �۱� ������ ���߱�

            // �ƴϸ� �� ���� ��ü�Ѵ�. 
            Swap(comparisonIndex, parentIndex);

            // �˻� ��ġ�� ����
            comparisonIndex = parentIndex;
        }
    }
    
    // ���� ���� ���� Pop : root ��
    public T Pop()
    {
        // ��ȯ�� ������ ���� ����
        T retData = heap[0];
        openNodeList.Remove(retData);

        // �� ���ġ ���� 
        // ������ �����͸� root�� �̵�
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];

        // ������ ������(�ε���) ����
        heap.RemoveAt(lastIndex);
        lastIndex--;

        // �ڽ�, �θ� ��� �� �� 
        int comparisonIndex = 0;
        while (true)
        {
            int leftIndex = 2 * comparisonIndex + 1;
            int rightIndex = 2 * comparisonIndex + 2;

            int nextIndex = comparisonIndex;

            // ���� : �ڽ� ����� �ε��� ���� ������ �ε����� �Ѿ�� �ʾƾ� �Ѵ�. 
            // �� ���� ���� ���簪���� ������, �������� �̵�
            if (leftIndex <= lastIndex && heap[nextIndex].CompareTo(heap[leftIndex]) > 0)
                nextIndex = leftIndex;
            // �� ������ ���� ���簪(�������� �̵��� �� ����)���� ������, ���������� �̵�
            // ( �̸� ����, ���� ��� ���� ���, �� ���� ���̶� ��ü�� �� �ְ� �ȴ�. )
            if (rightIndex <= lastIndex && heap[nextIndex].CompareTo(heap[rightIndex]) > 0)
                nextIndex = rightIndex;

            // �ڽ� ����� ��(����/������ �ڽ� ���) ��� ���� ������ ũ�� ����
            if (nextIndex == comparisonIndex)
                break;

            // �ƴϸ� �� ���� ��ü�Ѵ�. 
            Swap(comparisonIndex, nextIndex);

            // �˻� ��ġ�� �̵��Ѵ�. 
            comparisonIndex = nextIndex;
        }

        return retData;
    }

    private void Swap(int indexA, int indexB)
    {
        T temp = heap[indexA];
        heap[indexA] = heap[indexB];
        heap[indexB] = temp;
    }

    public int Count()
    {
        return heap.Count;
    }

    public bool Contains(T node)
    {
        return openNodeList.Contains(node);
    }
}
