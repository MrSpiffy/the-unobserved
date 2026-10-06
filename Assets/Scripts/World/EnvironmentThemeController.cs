using System.Collections.Generic;
using UnityEngine;
namespace Unobserved
{
    // A functional change requests a coherent new room palette. Each surface adopts
    // it only outside the camera frustum, preserving the feeling of a changed memory.
    // Physical collision never changes here; anchored entities and readable controls
    // are excluded. Authored floor regions commit atomically beyond the retention margin.
    public sealed class EnvironmentThemeController : MonoBehaviour
    {
        public EnvironmentThemeData[] themes;
        public Material wall,floor,accent;
        public readonly List<Light> lights=new List<Light>();
        public GameObject[] props;
        public int initialTheme;
        public int Index {get;private set;}
        public int AppliedSurfaceChanges {get;private set;}
        class Surface { public Renderer renderer; public Material original; public int palette; }
        SurfaceRevisionZone[] zones;
        readonly List<Surface> surfaces=new List<Surface>();
        readonly Dictionary<long,Material> variants=new Dictionary<long,Material>();
        float lastShift=-100,nextSample;
        StateDeck paletteDeck;
        bool occupied;
        ObservationTarget bounds;
        void Start()
        {
            Index=initialTheme; paletteDeck=new StateDeck(Mathf.RoundToInt(transform.position.x*37+transform.position.z*11)); bounds=GetComponent<ObservationTarget>(); bounds.FullyUnobserved+=RoomHidden; UnstableEntity.Shifted+=EntityShifted;
            zones=GetComponentsInChildren<SurfaceRevisionZone>();
            foreach(var renderer in GetComponentsInChildren<Renderer>())
            {
                if(renderer.GetComponentInParent<SurfaceRevisionZone>() || renderer.GetComponentInParent<DecorativeRevision>() || renderer.GetComponentInParent<SpatialRevision>() || !renderer.enabled || renderer.GetComponent<TextMesh>() || renderer.GetComponentInParent<UnstableEntity>() || renderer.GetComponentInParent<PuzzleRelay>()) continue;
                var imported=renderer.GetComponentInParent<ImportedVisual>();
                if(imported && !imported.structural) continue;
                string name=renderer.gameObject.name;
                if(name.Contains("Sign")||name.Contains("Wayfinding")||name.Contains("Landing edge")||name.Contains("Reserve edge")||name.Contains("shutter")||name.Contains("Shutter")) continue;
                if(!renderer.sharedMaterial || (!imported && !renderer.sharedMaterial.HasProperty("_Pattern"))) continue;
                if(!imported && renderer.sharedMaterial!=wall && name!="Surface tile" && name!="Wall surface") continue;
                surfaces.Add(new Surface{renderer=renderer,original=renderer.sharedMaterial,palette=Index});
            }
        }
        void EntityShifted(UnstableEntity entity) { if(entity.transform.IsChildOf(transform)) RequestShift(); }
        void RoomHidden()=>RequestShift();
        public void RequestShift()
        {
            // Several objects can leave view during one turn; they share one revision.
            // This does not delay or suppress any functional object transition.
            if(Time.time-lastShift<.65f) return;
            lastShift=Time.time; Index=paletteDeck.Next(themes.Length,Index);

        }
        void LateUpdate()
        {
            if(!GameFlow.Playing) return;
            var eye=GameFlow.Instance.Player.eye;
            bool inside=bounds.WorldBounds.Contains(GameFlow.Instance.Player.transform.position+Vector3.up);
            if(inside && !occupied) Soundscape.Instance.Theme(themes[Index].ambiencePitch);
            occupied=inside;
            if(inside)
            {
                var t=themes[Index]; RenderSettings.fogColor=Color.Lerp(RenderSettings.fogColor,t.fog,Time.deltaTime*1.5f);
                RenderSettings.fogDensity=Mathf.Lerp(RenderSettings.fogDensity,t.fogDensity,Time.deltaTime);
                eye.backgroundColor=RenderSettings.fogColor; Soundscape.Instance.Theme(t.ambiencePitch);
            }
            if(Time.time<nextSample) return; nextSample=Time.time+.055f;
            foreach(var surface in surfaces)
            {
                if(!surface.renderer || surface.palette==Index || ObservationManager.Instance.MaySee(surface.renderer.bounds,null,false)) continue;
                surface.renderer.sharedMaterial=Variant(surface.original,Index,surface.renderer.gameObject.name=="Surface tile");
                surface.palette=Index; AppliedSurfaceChanges++;
            }
            foreach(var zone in zones) if(zone.TryCommit(Index,FloorFinish(Index))) AppliedSurfaceChanges++;
            // Light influence extends beyond the fixture. Keep local illumination stable;
            // changing an offscreen bulb used to recolor surfaces still being watched.
        }

        public Material FloorFinish(int palette)=>Variant(floor,palette,true);

        Material Variant(Material original,int index,bool isFloor)
        {
            long key=((long)original.GetInstanceID()<<4)+(isFloor?8:0)+index;
            if(variants.TryGetValue(key,out var material)) return material;
            material=new Material(original); var theme=themes[index];
            if(isFloor)
            {
                material.shader=Shader.Find("Unobserved/Architectural Surface");
                material.color=index==0?new Color(.49f,.46f,.36f):index==1?new Color(.32f,.34f,.33f):new Color(.19f,.25f,.26f);
                SurfaceLibrary.Apply(material,index==0?1:index==1?0:4);
                material.SetFloat("_Glossiness",index==2?.42f:.18f);
                variants[key]=material; return material;
            }
            if(!original.HasProperty("_Pattern")) { material.color=original.color*(index==0?new Color(1,.98f,.91f):index==1?new Color(.81f,.89f,.94f):new Color(.9f,.76f,.6f)); variants[key]=material; return material; }
            int pattern=Mathf.RoundToInt(original.GetFloat("_Pattern"));
            Color color=original.color,emission=original.GetColor("_EmissionColor");
            if(emission.maxColorComponent>.2f) { material.color=theme.accent; material.SetColor("_EmissionColor",theme.accent*1.1f); }
            else if(pattern==5) { material.color=Color.Lerp(color,theme.accent*.16f,.5f); }
            else if(isFloor) { material.color=theme.floor; SurfaceLibrary.Apply(material,index==0?2:index==1?0:1); }
            else if(original==wall) { material.color=theme.walls; SurfaceLibrary.Apply(material,index==0?6:index==1?0:1); }
            else if(color.maxColorComponent<.13f) material.color=Color.Lerp(color,theme.walls*.18f,.3f);
            else
            {
                material.color=Color.Lerp(color,theme.walls,.55f);
                SurfaceLibrary.Apply(material,index==0?pattern:index==1?4:1);
            }
            variants[key]=material; return material;
        }
        void OnDestroy()
        {
            UnstableEntity.Shifted-=EntityShifted;
            foreach(var material in variants.Values) if(material) Destroy(material);
        }
    }
}
