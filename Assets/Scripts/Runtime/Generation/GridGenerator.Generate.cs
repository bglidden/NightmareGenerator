using UnityEngine;

namespace AHP
{
    public partial class GridGenerator
    {
        public LayoutGenerator LastLayout { get; private set; }

        [ContextMenu("AHP/Generate Layout")]
        public void GenerateLayout()
        {
            if (Config == null)
            {
                Debug.LogError("[AHP] GridConfig is missing. Cannot generate layout.");
                return;
            }

            InitRng();
            LastLayout = new LayoutGenerator(Config, Rng);
            LastLayout.Generate();

            Debug.Log($"[AHP] Layout generated: {LastLayout.Rooms.Count} rooms, Seed: {(Config.UseSeed ? Config.Seed.ToString() : "random")}");
        }
    }
}