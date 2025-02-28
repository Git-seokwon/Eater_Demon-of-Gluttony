using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using static XNode.Node;

[NodeWidth(300), NodeTint(60, 60, 60)]
public class LatentSkillSlotNode : XNode.Node
{
    [SerializeField]
    private string latentSkillName;

    [SerializeField]
    private int index;
    [SerializeField]
    private int level = 1;
    [SerializeField]
    private int maxLevel = 3;

    // 이 Node가 가지고 있는 Skill
    [SerializeField]
    private List<Skill> skill = new();

    [Output(connectionType = ConnectionType.Override), HideInInspector]
    [SerializeField]
    private LatentSkillSlotNode thisNode;


    public string LatentSkillName => latentSkillName;
    public int Index => index;
    public List<Skill> Skill => skill;
    public int Level => level;
    public bool IsMaxLevel => level >= maxLevel;

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

    public int LatentSkillLevelUp() => level = Mathf.Clamp(level + 1, 1, 3);

    public void SetLatentSkillLevel(int level) => this.level = level;
}
