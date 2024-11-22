using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using static XNode.Node;

[NodeWidth(300), NodeTint(60, 60, 60)]
public class LatentSkillSlotNode : XNode.Node
{
    [SerializeField]
    private int index;

    // �� Node�� ������ �ִ� Skill
    [SerializeField]
    private List<Skill> skill = new();

    [Output(connectionType = ConnectionType.Override), HideInInspector]
    [SerializeField]
    private LatentSkillSlotNode thisNode;

    public int Index => index;
    public List<Skill> Skill => skill;

    // Node�� ������� �� ���� 
    protected override void Init()
    {
        thisNode = this;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName != "thisNode")
            return null;
        return thisNode;
    }
}
