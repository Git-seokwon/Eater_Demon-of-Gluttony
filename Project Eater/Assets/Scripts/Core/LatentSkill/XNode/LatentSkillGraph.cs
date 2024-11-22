using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Latent Skill", menuName = "Skill/Latent Skill")]
public class LatentSkillGraph : XNode.NodeGraph
{
    public Dictionary<int, LatentSkillSlotNode> GetSlotNodes()
        => nodes.Where(x => x != null).Cast<LatentSkillSlotNode>().ToDictionary(node => (node.Index));
}
