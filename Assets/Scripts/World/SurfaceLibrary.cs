using System.Collections.Generic;
using UnityEngine;
namespace Unobserved
{
    public static class SurfaceLibrary
    {
        static readonly string[] names={"concrete","tile","carpet","wood","metal","water","plaster"};
        static readonly Dictionary<string,Texture2D> cache=new Dictionary<string,Texture2D>();
        static Texture2D Texture(string name)
        {
            if(!cache.TryGetValue(name,out var texture)) { texture=Resources.Load<Texture2D>("Surfaces/"+name); cache[name]=texture; }
            return texture;
        }
        public static void Apply(Material material,int pattern)
        {
            pattern=Mathf.Clamp(pattern,0,names.Length-1); string name=names[pattern];
            material.SetFloat("_Pattern",pattern);
            material.SetTexture("_BaseMap",Texture(name+"_albedo")); material.SetTexture("_BumpMap",Texture(name+"_normal")); material.SetTexture("_SurfaceMap",Texture(name+"_surface"));
            material.SetFloat("_TextureScale",pattern==2?1:pattern==1?.32f:.5f);
        }
    }
}
