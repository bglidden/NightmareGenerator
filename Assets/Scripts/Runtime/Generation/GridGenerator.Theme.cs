using UnityEngine;
using AHP.Themes;

namespace AHP
{
    public partial class GridGenerator
    {
        [Header("Theme")]
        public string ThemeName = "Asylum";
        public bool AutoApplyTheme = true;

        private HorrorTheme _currentTheme;

        public void LoadAndApplyTheme(string themeName)
        {
            _currentTheme = ThemeLoader.LoadFromResources(themeName);
            ApplyTheme();
        }

        [ContextMenu("AHP/Apply Theme")]
        public void ApplyTheme()
        {
            if (_currentTheme == null)
                _currentTheme = ThemeLoader.LoadFromResources(ThemeName);

            if (LastLayout == null)
            {
                Debug.LogWarning("[AHP] Generate layout before applying theme.");
                return;
            }

            // Apply lighting
            var lightingPass = new LightingPass(_currentTheme, PlacementRoot, Config, LastLayout, Rng);
            lightingPass.Apply();

            // Apply fog
            var fogPass = new FogPass(_currentTheme);
            fogPass.Apply();

            Debug.Log($"[AHP] Theme applied: {_currentTheme.ThemeName}");
        }
    }
}