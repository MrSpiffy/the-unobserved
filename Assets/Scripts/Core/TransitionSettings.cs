using UnityEngine;
namespace Unobserved
{
    [CreateAssetMenu(menuName = "Unobserved/Transition Settings")]
    public sealed class TransitionSettings : ScriptableObject
    {
        [HideInInspector] public float unseenDelay;
        [HideInInspector] public float observationDwell;
        [HideInInspector] public float sampleInterval;
        [Min(5)] public float maxDistance = 52;
    }
}
