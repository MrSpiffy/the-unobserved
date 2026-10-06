using UnityEngine;
namespace Unobserved
{
    public enum Form { Bridge, Steps, Pillar, Islands, Empty }
    [CreateAssetMenu(menuName = "Unobserved/Reality State")]
    public sealed class RealityState : ScriptableObject
    {
        public Form form;
        public Color color = new Color(.42f, .55f, .52f);
        public string displayName;
    }
}
