using UnityEngine;

namespace AHP
{
    public partial class GridGenerator
    {
        public LayoutGenerator LastLayout { get; private set; }
        public string LastLog { get; private set; }

        [ContextMenu("AHP/Generate Layout")]
        public void GenerateLayout()
        {
            if (Config == null)
            {
                Debug.LogError("[AHP] GridConfig is missing. Cannot generate layout.");
                return;
            }

            InitRng();

            var log = new GenerationLog();
            log.Info($"Grid Size: {Config.Width}x{Config.Height}");
            log.Info($"Seed: {(Config.UseSeed ? Config.Seed.ToString() : "random")}");

            LastLayout = new LayoutGenerator(Config, Rng);
            LastLayout.Generate();

            log.Info($"Rooms Generated: {LastLayout.Rooms.Count}");

            if (LastLayout.Rooms.Count < 3)
                log.Warn("Low room count. Consider increasing grid size or room attempts.");

            LastLog = log.ToString();
            Debug.Log($"[AHP] Generation Complete\n{LastLog}");
        }
    }
}