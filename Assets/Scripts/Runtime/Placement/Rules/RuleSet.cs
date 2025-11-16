using System.Collections.Generic;
using UnityEngine;

namespace AHP.Rules
{
    [CreateAssetMenu(fileName = "RuleSet", menuName = "AHP/Rules/Rule Set")]
    public class RuleSet : ScriptableObject
    {
        public List<PlacementRule> Rules = new List<PlacementRule>();

        public bool ValidatePlacement(PlacementContext context)
        {
            foreach (var rule in Rules)
            {
                if (!rule.Enabled) continue;
                if (!rule.Evaluate(context)) return false;
            }
            return true;
        }

        public float CalculateFinalProbability(PlacementContext context, float baseProbability)
        {
            float probability = baseProbability;
            
            foreach (var rule in Rules)
            {
                if (!rule.Enabled) continue;
                probability = rule.ModifyProbability(context, probability);
            }

            return Mathf.Clamp01(probability);
        }
    }
}