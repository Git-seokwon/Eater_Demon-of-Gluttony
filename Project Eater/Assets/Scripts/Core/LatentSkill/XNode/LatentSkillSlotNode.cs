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

    // 이 Node가 가지고 있는 Skill
    [SerializeField]
    private List<Skill> skill = new();

    [Output(connectionType = ConnectionType.Override), HideInInspector]
    [SerializeField]
    private LatentSkillSlotNode thisNode;

    public int Index => index;
    public List<Skill> Skill => skill;

    // Node가 만들어질 때 실행 
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
