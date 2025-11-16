using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AHP.Themes;

namespace AHP
{
    public class FogPass
    {
        private readonly HorrorTheme _theme;
        private Volume _globalVolume;

        public FogPass(HorrorTheme theme)
        {
            _theme = theme;
        }

        public void Apply()
        {
            if (!_theme.Fog.EnableFog)
            {
                RenderSettings.fog = false;
                return;
            }

            // Unity built-in fog (works with URP)
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = _theme.Fog.FogColor;
            RenderSettings.fogStartDistance = _theme.Fog.StartDistance;
            RenderSettings.fogEndDistance = _theme.Fog.EndDistance;

            // Optional: URP Volumetric Fog (if using Volume system)
            ConfigureVolumetricFog();
        }

        private void ConfigureVolumetricFog()
        {
            _globalVolume = Object.FindObjectOfType<Volume>();
            if (_globalVolume == null) return;

            // Note: URP doesn't have native volumetric fog like HDRP
            // But we can enhance with custom effects or local volumes
            Debug.Log("[AHP] Fog configured. For true volumetric fog, consider using local fog volumes.");
        }
    }
}