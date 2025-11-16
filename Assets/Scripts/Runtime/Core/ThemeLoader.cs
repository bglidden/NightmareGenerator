using UnityEngine;
using System.IO;

namespace AHP.Themes
{
    public static class ThemeLoader
    {
        private const string THEME_FOLDER = "Themes";

        /// <summary>
        /// Load theme from Resources/Themes/{themeName}.json
        /// </summary>
        public static HorrorTheme LoadFromResources(string themeName)
        {
            var textAsset = Resources.Load<TextAsset>($"{THEME_FOLDER}/{themeName}");
            
            if (textAsset == null)
            {
                Debug.LogError($"[AHP] Theme not found: {themeName}");
                return CreateDefaultTheme();
            }

            try
            {
                var theme = JsonUtility.FromJson<HorrorTheme>(textAsset.text);
                theme.Validate();
                Debug.Log($"[AHP] Loaded theme: {theme.ThemeName}");
                return theme;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AHP] Failed to parse theme {themeName}: {e.Message}");
                return CreateDefaultTheme();
            }
        }

        /// <summary>
        /// Load theme from arbitrary file path (editor/runtime)
        /// </summary>
        public static HorrorTheme LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[AHP] Theme file not found: {filePath}");
                return CreateDefaultTheme();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var theme = JsonUtility.FromJson<HorrorTheme>(json);
                theme.Validate();
                return theme;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AHP] Failed to load theme from {filePath}: {e.Message}");
                return CreateDefaultTheme();
            }
        }

        /// <summary>
        /// Save theme to file
        /// </summary>
        public static void SaveToFile(HorrorTheme theme, string filePath)
        {
            try
            {
                string json = JsonUtility.ToJson(theme, true);
                File.WriteAllText(filePath, json);
                Debug.Log($"[AHP] Theme saved to: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AHP] Failed to save theme: {e.Message}");
            }
        }

        private static HorrorTheme CreateDefaultTheme()
        {
            return new HorrorTheme
            {
                ThemeName = "Default Dark",
                Description = "Fallback theme",
                Colors = new HorrorTheme.ColorPalette
                {
                    FloorBase = new Color(0.15f, 0.15f, 0.15f),
                    WallBase = new Color(0.25f, 0.25f, 0.25f),
                    PropBase = new Color(0.3f, 0.3f, 0.3f),
                    AmbientLight = new Color(0.1f, 0.12f, 0.15f, 1f)
                },
                Lighting = new HorrorTheme.LightingSettings
                {
                    GlobalIntensity = 0.4f,
                    DirectionalColor = new Color(0.8f, 0.85f, 1f, 1f),
                    ShadowStrength = 0.8f,
                    BloomIntensity = 0.15f,
                    Exposure = 0.8f
                },
                Fog = new HorrorTheme.FogSettings
                {
                    EnableFog = true,
                    FogColor = new Color(0.05f, 0.05f, 0.08f),
                    Density = 0.02f,
                    StartDistance = 5f,
                    EndDistance = 40f
                },
                Audio = new HorrorTheme.AudioSettings
                {
                    AmbienceClipName = "",
                    AmbienceVolume = 0.3f,
                    TensionClipName = "",
                    TensionVolume = 0.2f
                }
            };
        }
    }
}