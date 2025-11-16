using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AHP.Themes;

namespace AHP
{
    public class LightingPass
    {
        private readonly HorrorTheme _theme;
        private readonly Transform _root;
        private readonly GridConfig _cfg;
        private readonly LayoutGenerator _layout;
        private readonly System.Random _rng;

        private Light _directionalLight;
        private Volume _globalVolume;

        public LightingPass(HorrorTheme theme, Transform root, GridConfig cfg, 
                           LayoutGenerator layout, System.Random rng)
        {
            _theme = theme;
            _root = root;
            _cfg = cfg;
            _layout = layout;
            _rng = rng;
        }

        public void Apply()
        {
            ConfigureAmbient();
            ConfigureDirectionalLight();
            ConfigurePostProcessing();
            SpawnRoomLights();
        }

        private void ConfigureAmbient()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = _theme.Colors.AmbientLight;
            RenderSettings.ambientIntensity = _theme.Lighting.GlobalIntensity;
        }

        private void ConfigureDirectionalLight()
        {
            _directionalLight = UnityEngine.Object.FindObjectOfType<Light>();
            
            if (_directionalLight == null)
            {
                var lightObj = new GameObject("Directional Light");
                lightObj.transform.SetParent(_root, false);
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                _directionalLight = lightObj.AddComponent<Light>();
                _directionalLight.type = LightType.Directional;
            }

            _directionalLight.color = _theme.Lighting.DirectionalColor;
            _directionalLight.intensity = _theme.Lighting.GlobalIntensity;
            _directionalLight.shadows = LightShadows.Soft;
            _directionalLight.shadowStrength = _theme.Lighting.ShadowStrength;

            // URP-specific
            var urpData = _directionalLight.GetComponent<UniversalAdditionalLightData>();
            if (urpData == null)
                urpData = _directionalLight.gameObject.AddComponent<UniversalAdditionalLightData>();
            
            // Set rendering layers in a version-safe way (handles URP/Unity API changes)
            SetLightRenderingLayer(_directionalLight, urpData, (uint)LightLayerEnum.LightLayerDefault);
        }

        // Handles API differences across URP/Unity versions without triggering obsolete or missing symbol errors
        private static void SetLightRenderingLayer(Light light, UniversalAdditionalLightData urpData, uint mask)
        {
            try
            {
                var ualdType = typeof(UniversalAdditionalLightData);

                // Preferred: UniversalAdditionalLightData.renderingLayerMask (newer URP)
                var prop = ualdType.GetProperty("renderingLayerMask");
                if (prop != null && prop.CanWrite)
                {
                    var value = System.Convert.ChangeType(mask, prop.PropertyType);
                    prop.SetValue(urpData, value, null);
                    return;
                }

                // Fallback (older API): UniversalAdditionalLightData.lightLayerMask
                prop = ualdType.GetProperty("lightLayerMask");
                if (prop != null && prop.CanWrite)
                {
                    var value = System.Convert.ChangeType(mask, prop.PropertyType);
                    prop.SetValue(urpData, value, null);
                    return;
                }

                // New Light API: Light.renderingLayers (Unity 2022.2+/6000+)
                var lightProp = typeof(Light).GetProperty("renderingLayers");
                if (lightProp != null && lightProp.CanWrite)
                {
                    var value = System.Convert.ChangeType(mask, lightProp.PropertyType);
                    lightProp.SetValue(light, value, null);
                    return;
                }
            }
            catch (System.Exception)
            {
                // Swallow exceptions to keep generation robust across editor/runtime differences.
            }
        }

        private void ConfigurePostProcessing()
        {
            _globalVolume = UnityEngine.Object.FindObjectOfType<Volume>();
            
            if (_globalVolume == null)
            {
                var volumeObj = new GameObject("Global Volume");
                volumeObj.transform.SetParent(_root, false);
                _globalVolume = volumeObj.AddComponent<Volume>();
                _globalVolume.isGlobal = true;
                _globalVolume.priority = 1;

                var profile = ScriptableObject.CreateInstance<VolumeProfile>();
                _globalVolume.profile = profile;
            }

            // Configure Bloom
            if (_globalVolume.profile.TryGet<Bloom>(out var bloom))
            {
                bloom.intensity.value = _theme.Lighting.BloomIntensity;
            }
            else
            {
                bloom = _globalVolume.profile.Add<Bloom>();
                bloom.intensity.value = _theme.Lighting.BloomIntensity;
                bloom.threshold.value = 0.9f;
                bloom.scatter.value = 0.7f;
            }

            // Configure Exposure (Tonemapping)
            if (_globalVolume.profile.TryGet<ColorAdjustments>(out var colorAdj))
            {
                colorAdj.postExposure.value = _theme.Lighting.Exposure;
            }
            else
            {
                colorAdj = _globalVolume.profile.Add<ColorAdjustments>();
                colorAdj.postExposure.value = _theme.Lighting.Exposure;
            }
        }

        private void SpawnRoomLights()
        {
            foreach (var room in _layout.Rooms)
            {
                // Spawn 1-3 lights per room based on size
                int lightCount = Mathf.Clamp(room.W * room.H / 30, 1, 3);

                for (int i = 0; i < lightCount; i++)
                {
                    int lx = room.X + _rng.Next(2, room.W - 2);
                    int ly = room.Y + _rng.Next(2, room.H - 2);

                    Vector3 pos = _cfg.CellToWorld(lx, ly);
                    pos.y = 2.5f + (float)_rng.NextDouble() * 0.5f;

                    var lightObj = new GameObject($"RoomLight_{lx}_{ly}");
                    lightObj.transform.SetParent(_root, false);
                    lightObj.transform.position = pos;

                    var light = lightObj.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.color = _theme.Lighting.DirectionalColor * 0.8f;
                    light.intensity = _theme.Lighting.GlobalIntensity * (0.5f + (float)_rng.NextDouble() * 0.5f);
                    light.range = 8f + (float)_rng.NextDouble() * 4f;
                    light.shadows = LightShadows.Soft;

                    // Add flickering component (created on Day 9)
                    var flicker = lightObj.AddComponent<LightFlicker>();
                    flicker.enabled = _rng.NextDouble() < 0.3;  // 30% of lights flicker
                }
            }
        }
    }
}