using UnityEngine;

[CreateAssetMenu]
// Category : Data나 여러 객체의 Tag 역할을 해줄 Data로 간단한 예로는 Skill이 공격 스킬인지, 회복 Skill인지 같은 것을 구별
//          : DataBase에서 관리하므로 IdentifiedObject를 상속
public class Category : IdentifiedObject
{
    // 같은 래퍼런스를 참조 하는 Equals
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }
    
    // Equals를 재정의 했기 때문에 GetHashCode도 재정의 해줘야 한다. 
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    // Category와 문자열(rhs)과의 비교 연산
    // ※ 비교 연산자(==) 오버로딩 
    public static bool operator ==(Category lhs, string rhs)
    { 
        if (lhs is null) // == null과 같은 기능이지만 속도가 더 빠르다.
            return rhs is null;
        return lhs.CodeName == rhs; // CodeName 문자열 비교
    }

    // 비교 연산자(==)를 오버로딩하면 짝이 되는 != 연산자도 오버로딩해야 한다. 
    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
}
