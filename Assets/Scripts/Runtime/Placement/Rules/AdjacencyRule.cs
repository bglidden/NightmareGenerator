using System.Collections.Generic;
using UnityEngine;

namespace AHP.Rules
{
    [CreateAssetMenu(fileName = "AdjacencyRule", menuName = "AHP/Rules/Adjacency Rule")]
    public class AdjacencyRule : PlacementRule
    {
        [System.Serializable]
        public struct Constraint
        {
            public PrefabCategory CategoryA;
            public PrefabCategory CategoryB;
            [Tooltip("Minimum cell distance required between A and B")]
            public int MinDistance;
        }

        public List<Constraint> Constraints = new List<Constraint>();

        public override bool Evaluate(PlacementContext context)
        {
            // Check against already-placed objects (tracked separately in PlacementPass)
            // For now, this is a placeholder - full implementation on Day 9
            return true;
        }
    }
}
