using UnityEngine;
namespace Unobserved
{
    // One authored landing is one atomic finish region, including its edge thickness.
    public sealed class SurfaceRevisionZone:MonoBehaviour
    {
        public Renderer[] surfaces;
        public Bounds WorldBounds {get;private set;}
        public int Palette {get;private set;}=-1;
        public int Commits {get;private set;}
        public void Prepare()
        {
            var selected=new System.Collections.Generic.List<Renderer>();
            foreach(var r in GetComponentsInChildren<Renderer>())
            {
                if(!r.enabled || r.name=="Landing edge") continue;
                var imported=r.GetComponentInParent<ImportedVisual>();
                if(imported && !imported.assetKey.Contains("Platform_")) continue;
                selected.Add(r);
            }
            surfaces=selected.ToArray(); var b=surfaces[0].bounds;
            foreach(var r in surfaces) b.Encapsulate(r.bounds); WorldBounds=b;
        }
        public bool TryCommit(int palette,Material material)
        {
            if(Palette==palette || ObservationManager.Instance.MaySee(WorldBounds,transform,false)) return false;
            foreach(var r in surfaces) r.sharedMaterial=material;
            foreach(var tag in GetComponentsInChildren<SurfaceTag>()) tag.kind=palette==2?SurfaceKind.Metal:SurfaceKind.Stone;
            Palette=palette; Commits++; return true;
        }
    }
}
