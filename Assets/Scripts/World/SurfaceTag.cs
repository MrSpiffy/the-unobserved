using UnityEngine;
namespace Unobserved
{
    public enum SurfaceKind { Carpet, Stone, Metal, Water }
    public sealed class SurfaceTag : MonoBehaviour { public SurfaceKind kind; }
}
