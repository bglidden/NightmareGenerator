using UnityEngine;

namespace AHP.Rules
{
    [CreateAssetMenu(fileName = "DensityRule", menuName = "AHP/Rules/Density Rule")]
    public class DensityRule : PlacementRule
    {
        [Header("Density Control")]
        [Tooltip("Probability curve based on distance from room center (X=distance, Y=probability)")]
        public AnimationCurve DensityCurve = AnimationCurve.Linear(0f, 1f, 10f, 0.1f);

        [Tooltip("Which categories does this rule apply to?")]
        public PrefabCategory[] ApplicableCategories = { PrefabCategory.Prop };

        [Header("Room-Specific")]
        [Tooltip("Apply only to rooms (ignore corridors)")]
        public bool RoomsOnly = true;

        public override bool Evaluate(PlacementContext context)
        {
            // Density rules don't block placement, just modify probability
            return true;
        }

        public override float ModifyProbability(PlacementContext context, float baseProbability)
        {
            if (!Enabled) return baseProbability;

            // Check if category matches
            bool applies = false;
            foreach (var cat in ApplicableCategories)
            {
                if (cat == context.Category)
                {
                    applies = true;
                    break;
                }
            }
            if (!applies) return baseProbability;

            // Check room-only constraint
            if (RoomsOnly && context.CellType != CellType.Room)
                return baseProbability;

            float distance = context.DistanceToNearestRoomCenter();
            float multiplier = DensityCurve.Evaluate(distance);

            return baseProbability * multiplier;
        }
    }
}