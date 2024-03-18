using UnityEngine;

[CreateAssetMenu]
// Category : Data�� ���� ��ü�� Tag ������ ���� Data�� ������ ���δ� Skill�� ���� ��ų����, ȸ�� Skill���� ���� ���� ����
//          : DataBase���� �����ϹǷ� IdentifiedObject�� ���
public class Category : IdentifiedObject
{
    // ���� ���۷����� ���� �ϴ� Equals
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }
    
    // Equals�� ������ �߱� ������ GetHashCode�� ������ ����� �Ѵ�. 
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    // Category�� ���ڿ�(rhs)���� �� ����
    // �� �� ������(==) �����ε� 
    public static bool operator ==(Category lhs, string rhs)
    { 
        if (lhs is null) // == null�� ���� ��������� �ӵ��� �� ������.
            return rhs is null;
        return lhs.CodeName == rhs; // CodeName ���ڿ� ��
    }

    // �� ������(==)�� �����ε��ϸ� ¦�� �Ǵ� != �����ڵ� �����ε��ؾ� �Ѵ�. 
    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
}
