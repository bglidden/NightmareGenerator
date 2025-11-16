using UnityEngine;

namespace AHP.Themes
{
    [CreateAssetMenu(fileName = "ThemeAsset", menuName = "AHP/Configs/Theme Asset")]
    public class ThemeAsset : ScriptableObject
    {
        public HorrorTheme Theme;

        private void OnValidate()
        {
            Theme.Validate();
        }

        // Export to JSON
        public string ToJson()
        {
            return JsonUtility.ToJson(Theme, true);
        }

        // Import from JSON
        public void FromJson(string json)
        {
            Theme = JsonUtility.FromJson<HorrorTheme>(json);
            Theme.Validate();
        }
    }
}