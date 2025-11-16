using UnityEngine;
using System;

namespace AHP.Themes
{
    [Serializable]
    public class HorrorTheme
    {
        [Header("Theme Identity")]
        public string ThemeName = "Unnamed Theme";
        public string Description = "";

        [Header("Color Palette")]
        public ColorPalette Colors;

        [Header("Lighting")]
        public LightingSettings Lighting;

        [Header("Fog Settings")]
        public FogSettings Fog;

        [Header("Ambient Audio")]
        public AudioSettings Audio;

        [Serializable]
        public struct ColorPalette
        {
            [ColorUsage(false)] public Color FloorBase;
            [ColorUsage(false)] public Color WallBase;
            [ColorUsage(false)] public Color PropBase;
            [ColorUsage(true, true)] public Color AmbientLight;  // HDR for bloom
        }

        [Serializable]
        public struct LightingSettings
        {
            [Range(0f, 8f)] public float GlobalIntensity;
            [ColorUsage(true, true)] public Color DirectionalColor;
            [Range(0f, 1f)] public float ShadowStrength;
            [Range(0f, 5f)] public float BloomIntensity;
            [Range(0f, 2f)] public float Exposure;
        }

        [Serializable]
        public struct FogSettings
        {
            public bool EnableFog;
            [ColorUsage(false)] public Color FogColor;
            [Range(0f, 1f)] public float Density;
            [Range(0f, 100f)] public float StartDistance;
            [Range(0f, 200f)] public float EndDistance;
        }

        [Serializable]
        public struct AudioSettings
        {
            public string AmbienceClipName;
            [Range(0f, 1f)] public float AmbienceVolume;
            public string TensionClipName;
            [Range(0f, 1f)] public float TensionVolume;
        }

        // Validation
        public void Validate()
        {
            Lighting.GlobalIntensity = Mathf.Clamp(Lighting.GlobalIntensity, 0f, 8f);
            Lighting.ShadowStrength = Mathf.Clamp01(Lighting.ShadowStrength);
            Fog.Density = Mathf.Clamp01(Fog.Density);
            Audio.AmbienceVolume = Mathf.Clamp01(Audio.AmbienceVolume);
            Audio.TensionVolume = Mathf.Clamp01(Audio.TensionVolume);
        }
    }
}