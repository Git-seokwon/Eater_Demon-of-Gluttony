using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu]
public class IdentifiedObject : ScriptableObject, ICloneable // ICloneable : ��ü ���縦 ���� �������̽�
{
    #region Category
    // Category �迭 : IdentifiedObject�� ��ӹ޴� Data���� ���� Tag(Category)�� ���� �� �ִ�.
    [SerializeField]
    private Category[] categories; 
    #endregion

    #region Private Data
    [SerializeField]
    private Sprite icon; 
    [SerializeField]
    private int id = -1; // id�� �ڵ忡�� �Ҵ�
    [SerializeField]
    private string codeName; 
    [SerializeField]
    private string displayName; // �̸�
    [SerializeField]
    private string description; // ����
    [SerializeField]
    private string specificDescription; // �� ���� (��ų �� ����)
    #endregion

    // ������ ���� �������� �ܺο� �����ֱ� ���� ������Ƽ (�б� �����̹Ƿ� �ܺο��� ���� ������ �� ����)
    // "=>" lambda expression�� ����Ͽ� ���Ϲ��� ���� �޼��峪 �Ӽ��� �� �����ϰ� �ۼ��� �� �ִ�.
    #region Private Data -> Public
    public Sprite Icon => icon; // public Sprite Icon { get { return icon; } }
    public int ID => id;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    // Description�� �ڽ� Class���� �ʿ信 ���� ������ ������ �� �ֵ��� virtual�� ����
    public virtual string Description => description;
    public virtual string SpecificDescription => specificDescription;
    #endregion

    // Clone �Լ��� ICloneable �������̽��� �������� IdentifiedObject�� �����ϰ� �����ϴ� �Լ�
    // IdentifiedObject�� ��ӹ޴� Class���� �ش� �Լ��� �������Ͽ� �ڽŵ��� Clone�� ���� �� �ֵ��� ���� �Լ��� ����
    // �� Prototype Pattern : ��ü�� �θ� Type���� Upcasting �� ������ ��, ���� �ڷ����� ���� ���纻�� ����
    public virtual object Clone() => Instantiate(this);

    // Data�� Ư�� Category�� ������ �ִ��� Ȯ���ϴ� �Լ���
    // Enumerable.Any : ���ǿ� �ش��ϴ� ���� 1���� �����Ѵٸ�, true�� ��ȯ
    #region Category Check
    // �Ű� ���� category�� ID�� categories�� �迭 ������ ID�� ���Ѵ�. 
    // == ���� Tag�� �ϳ��� ������ �ִ��� Ȯ��
    public bool HasCategory(Category category)
        => categories.Any(x => x.ID == category.ID);

    // ���ڿ��� categories�� ������ CodeName�� ��
    // public static bool operator ==(Category lhs, string rhs)
    public bool HasCategory(string category)
        => categories.Any(x => x == category);
    #endregion
}
