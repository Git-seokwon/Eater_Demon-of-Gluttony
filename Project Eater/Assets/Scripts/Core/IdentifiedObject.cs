using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu]
public class IdentifiedObject : ScriptableObject, ICloneable // ICloneable : 객체 복사를 위한 인터페이스
{
    #region Category
    // Category 배열 : IdentifiedObject를 상속받는 Data들은 여러 Tag(Category)를 가질 수 있다.
    [SerializeField]
    private Category[] categories; 
    #endregion

    #region Private Data
    [SerializeField]
    private Sprite icon; 
    [SerializeField]
    private int id = -1; // id는 코드에서 할당
    [SerializeField]
    private string codeName; 
    [SerializeField]
    private string displayName; // 이름
    [SerializeField]
    private string description; // 설명
    [SerializeField]
    private string specificDescription; // 상세 설명 (스킬 상세 설명)
    #endregion

    // 위에서 만든 변수들을 외부에 보여주기 위한 프로퍼티 (읽기 전용이므로 외부에서 값을 설정할 수 없음)
    // "=>" lambda expression을 사용하여 단일문을 가진 메서드나 속성을 더 간결하게 작성할 수 있다.
    #region Private Data -> Public
    public Sprite Icon => icon; // public Sprite Icon { get { return icon; } }
    public int ID => id;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    // Description은 자식 Class에서 필요에 따라 내용을 수정할 수 있도록 virtual로 선언
    public virtual string Description => description;
    public virtual string SpecificDescription => specificDescription;
    #endregion

    // Clone 함수는 ICloneable 인터페이스의 구현으로 IdentifiedObject를 간단하게 복사하는 함수
    // IdentifiedObject를 상속받는 Class들이 해당 함수를 재정의하여 자신들의 Clone을 만들 수 있도록 가상 함수로 만듬
    // ※ Prototype Pattern : 객체가 부모 Type으로 Upcasting 된 상태일 때, 원래 자료형을 몰라도 복사본을 생성
    public virtual object Clone() => Instantiate(this);

    // Data가 특정 Category를 가지고 있는지 확인하는 함수들
    // Enumerable.Any : 조건에 해당하는 값이 1개라도 존재한다면, true를 반환
    #region Category Check
    // 매개 변수 category의 ID와 categories의 배열 원소의 ID를 비교한다. 
    // == 같은 Tag를 하나라도 가지고 있는지 확인
    public bool HasCategory(Category category)
        => categories.Any(x => x.ID == category.ID);

    // 문자열과 categories의 원소의 CodeName을 비교
    // public static bool operator ==(Category lhs, string rhs)
    public bool HasCategory(string category)
        => categories.Any(x => x == category);
    #endregion
}
