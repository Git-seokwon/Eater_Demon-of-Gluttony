using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatentSkill : IdentifiedObject
{
    [SerializeField, HideInInspector]
    private LatentSkillGraph graph;

    public Dictionary<int, LatentSkillSlotNode> GetSlotNodes()
        => graph.GetSlotNodes();
}
