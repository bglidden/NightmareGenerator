using System.Collections.Generic;
using UnityEngine;

namespace AHP
{
    [CreateAssetMenu(fileName = "PrefabSet", menuName = "AHP/Configs/PrefabSet")]
    public class PrefabSet : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public PrefabCategory Category;
            public GameObject Prefab;
            [Range(0f, 1f)] public float Weight;
        }

        public List<Entry> Items = new List<Entry>();

        public GameObject GetRandom(System.Random rng, PrefabCategory category)
        {
            var candidates = Items.FindAll(item => 
                item.Category == category && 
                item.Prefab != null && 
                item.Weight > 0f);

            if (candidates.Count == 0)
            {
                Debug.LogWarning($"[AHP] No prefabs found for category: {category}");
                return null;
            }

            // Weighted random selection
            float totalWeight = 0f;
            foreach (var candidate in candidates)
                totalWeight += candidate.Weight;

            float roll = (float)rng.NextDouble() * totalWeight;
            float cumulative = 0f;

            foreach (var candidate in candidates)
            {
                cumulative += candidate.Weight;
                if (roll <= cumulative)
                    return candidate.Prefab;
            }

            return candidates[candidates.Count - 1].Prefab;
        }

        private void OnValidate()
        {
            for (int i = 0; i < Items.Count; i++) // or Items.Length for arrays
            {
                var v = Items[i];
                if (v.Weight < 0f) v.Weight = 0f;
                if (v.Weight > 1f) v.Weight = 1f;
                Items[i] = v; // write the modified copy back
            }
            /*
            // Normalize weights if needed
            foreach (var item in Items)
            {
                if (item.Weight < 0f) item.Weight = 0f;
                if (item.Weight > 1f) item.Weight = 1f;
            }
            */
        }
    }
}