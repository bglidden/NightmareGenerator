using UnityEngine;
using AHP.Rules;

namespace AHP
{
    public partial class GridGenerator
    {
        [Header("Placement")]
        public PrefabSet Prefabs;
        public RuleSet Rules;
        public Transform PlacementRoot;
        public bool SpawnWalls = true;

        [ContextMenu("AHP/Build Placement")]
        public void BuildPlacement()
        {
            if (Config == null || LastLayout == null)
            {
                Debug.LogError("[AHP] Generate layout first before building placement.");
                return;
            }

            if (Prefabs == null)
            {
                Debug.LogError("[AHP] PrefabSet is missing. Assign a prefab set.");
                return;
            }

            if (PlacementRoot == null)
            {
                var rootObj = new GameObject("~Generated");
                rootObj.transform.SetParent(transform, false);
                PlacementRoot = rootObj.transform;
            }

            var pass = new PlacementPass(Config, Rng, LastLayout, Prefabs, PlacementRoot, Rules);
            pass.Build(SpawnWalls);

            Debug.Log("[AHP] Placement complete.");
        }
    }
}