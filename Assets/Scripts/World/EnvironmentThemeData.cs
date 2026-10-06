using UnityEngine;
namespace Unobserved
{
    [CreateAssetMenu(menuName = "Unobserved/Environment Theme")]
    public sealed class EnvironmentThemeData : ScriptableObject
    {
        public Color walls, floor, accent, fog;
        public float fogDensity = .012f;
        public float ambiencePitch = 1;
        public float lightIntensity = 1;
    }
}
