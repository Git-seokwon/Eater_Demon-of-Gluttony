using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/QCategory", fileName ="QCategory_")]

public class QCategory : ScriptableObject, IEquatable<QCategory>
{
    [SerializeField] private string codeName;
    [SerializeField] private string displayName;

    public string CodeName => codeName;
    public string DisplayName => displayName;

    #region Operator
    public bool Equals(QCategory other)
    {
        if(other is null)
            return false;
        if (ReferenceEquals(other, this))
            return true;
        if(this.GetType() != other.GetType()) 
            return false;

        return CodeName == other.CodeName;
    }

    public override int GetHashCode() => (CodeName, DisplayName).GetHashCode();

    public override bool Equals(object other) => base.Equals(other);

    public static bool operator ==(QCategory lhs, string rhs)
    {
        if (lhs is null)
            return ReferenceEquals(rhs, null);
        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }

    public static bool operator !=(QCategory lhs, string rhs) => !(lhs == rhs);
    #endregion
}
