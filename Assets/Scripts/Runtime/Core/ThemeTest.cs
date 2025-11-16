using UnityEngine;
using AHP.Themes;

namespace AHP
{
    public class ThemeTest : MonoBehaviour
    {
        [ContextMenu("Load Asylum Theme")]
        void LoadAsylum()
        {
            var theme = ThemeLoader.LoadFromResources("Asylum");
            Debug.Log($"Loaded: {theme.ThemeName} - {theme.Description}");
            Debug.Log($"Fog enabled: {theme.Fog.EnableFog}, Density: {theme.Fog.Density}");
        }

        [ContextMenu("Load Hospital Theme")]
        void LoadHospital()
        {
            var theme = ThemeLoader.LoadFromResources("AbandonedHospital");
            Debug.Log($"Loaded: {theme.ThemeName}");
        }

        [ContextMenu("Load Manor Theme")]
        void LoadManor()
        {
            var theme = ThemeLoader.LoadFromResources("CursedManor");
            Debug.Log($"Loaded: {theme.ThemeName}");
        }
    }
}