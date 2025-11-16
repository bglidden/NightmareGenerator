using UnityEngine;

namespace AHP
{
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [Header("Flicker Settings")]
        [Range(0f, 1f)] public float FlickerIntensity = 0.3f;
        [Range(0.01f, 0.5f)] public float FlickerSpeed = 0.1f;
        public bool RandomStart = true;

        private Light _light;
        private float _baseIntensity;
        private float _timer;
        private float _nextFlickerTime;

        void Awake()
        {
            _light = GetComponent<Light>();
            _baseIntensity = _light.intensity;
            
            if (RandomStart)
                _timer = Random.Range(0f, 1f);
        }

        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _nextFlickerTime)
            {
                float flicker = Random.Range(-FlickerIntensity, FlickerIntensity);
                _light.intensity = Mathf.Max(0f, _baseIntensity + flicker);
                
                _nextFlickerTime = _timer + FlickerSpeed + Random.Range(0f, FlickerSpeed);
            }
        }
    }
}